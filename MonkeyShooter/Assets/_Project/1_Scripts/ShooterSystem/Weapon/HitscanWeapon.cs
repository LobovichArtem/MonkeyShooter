using System;
using System.Collections.Generic;
using UnityEngine;

public class HitscanWeapon
{
    private readonly WeaponData _weaponData;
    private readonly Transform _aimPoint;

    private int _currentAmmo;
    private float _nextFireTime;
    private float _reloadStartTime;

    public event Action OnShot;
    public event Action<HitscanHitInfo> OnHit;
    public event Action OnReloadStarted;
    public event Action OnReloadFinished;
    public event Action OnEmptyClipAttempt;

    public int CurrentAmmo => _currentAmmo;
    public int MagazineSize => _weaponData.MagazineSize;
    public bool IsReloading => _reloadStartTime > 0f && Time.time < _reloadStartTime + _weaponData.ReloadTime;

    private const int _MAXHITS = 3;
    private readonly RaycastHit[] _hitsBuffer = new RaycastHit[_MAXHITS];
    private LayerMask _hitscanLayer;

    public HitscanWeapon(WeaponData data, Transform aimPoint)
    {
        _weaponData = data ?? throw new ArgumentNullException(nameof(data));
        _currentAmmo = _weaponData.MagazineSize;
        _aimPoint = aimPoint;
        _hitscanLayer = GlobalWeaponSettings.Instance.WeaponHitscanLayer;
    }

    public void ProcessInput(in CombatFrameInput input)
    {
        if (_reloadStartTime > 0f && !IsReloading)
        {
            _currentAmmo = _weaponData.MagazineSize;
            _reloadStartTime = 0f;
            OnReloadFinished?.Invoke();
        }

        if (IsReloading)
            return;

        if (input.IsReloadRequested && _currentAmmo < _weaponData.MagazineSize)
        {
            StartReload();
            return;
        }

        bool wantsToFire = _weaponData.IsAutomatic ? input.IsFireHeld : input.IsFirePressed;

        if (wantsToFire && Time.time >= _nextFireTime)
        {
            if (_currentAmmo > 0)
            {
                Shoot(_aimPoint.position, _aimPoint.forward);
            }
            else
            {
                OnEmptyClipAttempt?.Invoke();
                StartReload();
            }
        }
    }

    public void Shoot(Vector3 origin, Vector3 direction)
    {
        _nextFireTime = Time.time + _weaponData.FireDelay;
        _currentAmmo--;

        OnShot?.Invoke();

        int pelletCount = Mathf.Max(1, _weaponData.PelletCount);
        float damagePerPellet = _weaponData.BaseDamage / pelletCount;
        float forcePerPellet = _weaponData.ImpactForce / pelletCount;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 pelletDirection = direction;

            if (_weaponData.SpreadAngle > 0f)
            {
                pelletDirection = GetSpreadDirection(direction, _weaponData.SpreadAngle);
            }

            ExecuteSinglePellet(origin, pelletDirection, damagePerPellet, forcePerPellet);
        }
    }

    private void ExecuteSinglePellet(Vector3 origin, Vector3 direction, float initialDamage, float initialForce)
    {
        float currentPenetration = _weaponData.Penetration;
        float currentDamage = initialDamage;

        // Выполняем RaycastNonAlloc для получения всех попаданий луча за один вызов
        int hitCount = Physics.RaycastNonAlloc(origin, direction, _hitsBuffer, _weaponData.MaxDistance, _hitscanLayer);
        if (hitCount == 0)
            return;

        // Сортируем полученные хиты по расстоянию от точки выстрела, так как RaycastNonAlloc не гарантирует порядок
        System.Array.Sort(_hitsBuffer, 0, hitCount, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

        // Рассчитываем начальный урон с учетом расстояния до первого попадания
        float distance = _hitsBuffer[0].distance;
        int pelletCount = Mathf.Max(1, _weaponData.PelletCount);
        currentDamage = CalculateDamageByDistance(distance) / pelletCount;

        // Коллекция для отслеживания уже пораженных персонажей в рамках этой пеллеты
        HashSet<Transform> hitCharacters = new HashSet<Transform>();

        int processedHits = 0;
        for (int i = 0; i < hitCount && processedHits < _MAXHITS && currentPenetration > 0f; i++)
        {
            var hit = _hitsBuffer[i];
            processedHits++;

            Transform hitRoot = hit.collider.transform.root;
            bool hasDamageable = hit.collider.TryGetComponent<IDamageable>(out var damageable);
            bool hasProtectable = hit.collider.TryGetComponent<IProtectable>(out var protectable);
            bool hasKnockbackable = hit.collider.TryGetComponent<IKnockbackable>(out var knockbackable);

            // Если попали в стену или объект без здоровья
            if (!hasDamageable && !hasProtectable)
            {
                HitscanHitInfo wallHitInfo = new HitscanHitInfo(hit.point, hit.normal, hit.collider);
                OnHit?.Invoke(wallHitInfo);

                if (hasKnockbackable)
                {
                    Vector3 wallImpulse = direction * initialForce;
                    knockbackable.AddImpulse(wallImpulse, hit.point);
                }
                break;
            }

            // Если этот персонаж уже получал урон от этой пеллеты, пропускаем нанесение урона, но даем пуле лететь дальше
            if (hitCharacters.Contains(hitRoot))
            {
                continue;
            }

            hitCharacters.Add(hitRoot);

            HitscanHitInfo hitInfo = new HitscanHitInfo(hit.point, hit.normal, hit.collider);
            OnHit?.Invoke(hitInfo);

            float armorValue = hasProtectable ? protectable.ArmorValue : 0f;
            float damageToApply = currentDamage;

            if (armorValue > 0f)
            {
                float remainingPenetration = currentPenetration - armorValue;

                if (remainingPenetration <= 0f)
                {
                    if (hasKnockbackable)
                    {
                        Vector3 blockedImpulse = direction * initialForce;
                        knockbackable.AddImpulse(blockedImpulse, hit.point);
                    }
                    break;
                }

                float damageFactor = remainingPenetration / currentPenetration;
                damageToApply = currentDamage * damageFactor;
                currentPenetration = remainingPenetration;
            }

            if (hasDamageable)
            {
                damageable.TakeDamage(damageToApply, hit.point, hit.normal);
            }

            if (hasKnockbackable)
            {
                float impulseFactor = currentDamage > 0f ? (damageToApply / currentDamage) : 1f;
                Vector3 impulseForce = direction * (initialForce * impulseFactor);
                knockbackable.AddImpulse(impulseForce, hit.point);
            }

            currentDamage = damageToApply;
        }
    }

    private Vector3 GetSpreadDirection(Vector3 baseDirection, float spreadAngleDegrees)
    {
        Quaternion spreadRotation = Quaternion.Euler(
            UnityEngine.Random.Range(-spreadAngleDegrees, spreadAngleDegrees),
            UnityEngine.Random.Range(-spreadAngleDegrees, spreadAngleDegrees),
            0f
        );
        return spreadRotation * baseDirection;
    }

    private void StartReload()
    {
        _reloadStartTime = Time.time;
        OnReloadStarted?.Invoke();
    }

    private float CalculateDamageByDistance(float distance)
    {
        if (distance <= _weaponData.EffectiveDistance)
        {
            return _weaponData.BaseDamage;
        }

        return _weaponData.MinDamage;

        // Линейное падение от EffectiveRange до MaxDistance
        float rangeDelta = _weaponData.MaxDistance - _weaponData.EffectiveDistance;
        if (rangeDelta <= 0f) return _weaponData.MinDamage;

        float distanceBeyondEffective = distance - _weaponData.EffectiveDistance;
        float t = Mathf.Clamp01(distanceBeyondEffective / rangeDelta);

        return Mathf.Lerp(_weaponData.BaseDamage, _weaponData.MinDamage, t);
    }
}