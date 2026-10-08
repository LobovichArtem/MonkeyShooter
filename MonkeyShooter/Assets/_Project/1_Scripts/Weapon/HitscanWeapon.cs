using System;
using UnityEditor.PackageManager;
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


    public HitscanWeapon(WeaponData data, Transform aimPoint)
    {
        _weaponData = data ?? throw new ArgumentNullException(nameof(data));
        _currentAmmo = _weaponData.MagazineSize;
        _aimPoint = aimPoint;
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

        float currentPenetration = _weaponData.Penetration;
        float currentDamage = _weaponData.BaseDamage;
        Vector3 currentRayOrigin = origin;

        int maxHits = 3;

        while (currentPenetration > 0f && maxHits > 0)
        {
            maxHits--;

            if (!Physics.Raycast(currentRayOrigin, direction, out RaycastHit hit, _weaponData.MaxDistance))
            {
                break; // Пуля улетела в воздух
            }

            // 1. Уведомляем о каждом отдельном попадании
            HitscanHitInfo hitInfo = new HitscanHitInfo(hit.point, hit.normal, hit.collider);
            OnHit?.Invoke(hitInfo);

            // 2. Получаем значение брони цели
            float armorValue = 0f;
            if (hit.collider.TryGetComponent(out IProtectable protectable))
            {
                armorValue = protectable.ArmorValue;
            }

            float damageToApply = currentDamage;

            // 3. Расчет пробития и урона
            if (armorValue > 0f)
            {
                float remainingPenetration = currentPenetration - armorValue;

                if (remainingPenetration <= 0f)
                {
                    // Броня оказалась сильнее пули — пробития нет
                    break;
                }

                // Пробитие есть! Урон зависит от остатка пробиваемости
                float damageFactor = remainingPenetration / currentPenetration;
                damageToApply = currentDamage * damageFactor;

                // Обновляем остаточную пробиваемость для выстрела насквозь
                currentPenetration = remainingPenetration;
            }

            // 4. Наносим урон
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damageToApply, hit.point, hit.normal);
            }

            // 5. Применяем физический импульс
            if (hit.collider.TryGetComponent(out IKnockbackable knockbackable))
            {
                Vector3 impulseForce = direction * (_weaponData.ImpactForce * (damageToApply / currentDamage));
                knockbackable.AddImpulse(impulseForce, hit.point);
            }

            // 6. Урон пули для следующей цели за преградой становится равным пропущенному урону
            currentDamage = damageToApply;

            // Сдвигаем точку следующего Raycast за точку вхождения
            currentRayOrigin = hit.point + direction * 0.01f;
        }
    }

    private void StartReload()
    {
        _reloadStartTime = Time.time;
        OnReloadStarted?.Invoke();
    }
}