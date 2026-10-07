using System;
using UnityEngine;

public class HitscanWeapon
{
    private readonly WeaponData _data;

    private int _currentAmmo;
    private float _nextFireTime;
    private float _reloadStartTime;

    public event Action<HitscanHitInfo> OnShot;
    public event Action OnReloadStarted;
    public event Action OnReloadFinished;
    public event Action OnEmptyClipAttempt;

    public int CurrentAmmo => _currentAmmo;
    public int MagazineSize => _data.MagazineSize;
    public bool IsReloading => _reloadStartTime > 0f && Time.time < _reloadStartTime + _data.ReloadTime;


    public HitscanWeapon(WeaponData data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _currentAmmo = _data.MagazineSize;
    }

    public void ProcessInput(in CombatFrameInput input, Transform cameraTransform)
    {
        if (_reloadStartTime > 0f && !IsReloading)
        {
            _currentAmmo = _data.MagazineSize;
            _reloadStartTime = 0f;
            OnReloadFinished?.Invoke();
        }

        if (IsReloading)
            return;

        if (input.IsReloadRequested && _currentAmmo < _data.MagazineSize)
        {
            StartReload();
            return;
        }

        bool wantsToFire = _data.IsAutomatic ? input.IsFireHeld : input.IsFirePressed;

        if (wantsToFire && Time.time >= _nextFireTime)
        {
            if (_currentAmmo > 0)
            {
                Shoot(cameraTransform);
            }
            else
            {
                OnEmptyClipAttempt?.Invoke();
                StartReload();
            }
        }
    }

    private void Shoot(Transform cameraTransform)
    {
        _nextFireTime = Time.time + _data.FireDelay;
        _currentAmmo--;

        Vector3 shootDirection = cameraTransform.forward;
        Vector3 origin = cameraTransform.position;

        HitscanHitInfo hitInfo;

        if (Physics.Raycast(origin, shootDirection, out RaycastHit hit, _data.MaxDistance))
        {
            float damage = hit.distance <= _data.EffectiveDistance ? _data.BaseDamage : _data.MinDamage;

            if (hit.collider.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(damage, hit.point, hit.normal);
            }

            hitInfo = new HitscanHitInfo(true, origin, hit.point);
        }
        else
        {
            hitInfo = new HitscanHitInfo(false, origin, Vector3.zero);
        }

        OnShot?.Invoke(hitInfo);
    }



    private void StartReload()
    {
        _reloadStartTime = Time.time;
        OnReloadStarted?.Invoke();
    }
}