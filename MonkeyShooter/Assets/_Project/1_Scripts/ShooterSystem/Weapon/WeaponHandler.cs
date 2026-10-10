using System;
using UnityEngine;

public class WeaponHandler
{
    private readonly Transform _cameraTransform;
    private readonly Transform _weaponContainer; // Точка-пустышка у камеры (куда спавнится модель)

    private HitscanWeapon _currentWeaponLogic;
    private WeaponView _currentWeaponView;

    public HitscanWeapon CurrentWeaponLogic => _currentWeaponLogic;
    public WeaponView CurrentWeaponView => _currentWeaponView;

    public event Action<HitscanWeapon> OnWeaponChanged;

    private WeaponEffectsService _effectsService;

    private CameraRecoil _cameraRecoil;

    public WeaponHandler(Transform cameraTransform, Transform weaponContainer)
    {
        _cameraTransform = cameraTransform ?? throw new ArgumentNullException(nameof(cameraTransform));
        _weaponContainer = weaponContainer ?? throw new ArgumentNullException(nameof(weaponContainer));
        _effectsService = ServiceLocator.Get<WeaponEffectsService>();
        _cameraRecoil = new CameraRecoil(cameraTransform);
    }

    public void EquipWeapon(WeaponData newWeaponData)
    {
        if (newWeaponData == null)
            return;

        UnsubscribeCurrentWeapon();

        if (_currentWeaponView != null)
        {
            UnityEngine.Object.Destroy(_currentWeaponView.gameObject);
        }

        _currentWeaponLogic = new HitscanWeapon(newWeaponData, _cameraTransform);

        if (newWeaponData.WeaponViewPrefab != null)
        {
            _currentWeaponView = UnityEngine.Object.Instantiate(newWeaponData.WeaponViewPrefab, _weaponContainer);
            _currentWeaponView.transform.localPosition = Vector3.zero;
            _currentWeaponView.transform.localRotation = Quaternion.identity;

            // Инициализируем конфиг отдачи визуала из данных оружия
            _currentWeaponView.Init(newWeaponData.KickConfig);
        }

        SubscribeCurrentWeapon();
        // Обновляем конфиг отдачи камеры
        _cameraRecoil?.SetConfig(newWeaponData.RecoilConfig);
    }

    public void ProcessInput(in CombatFrameInput combatInput)
    {
        _currentWeaponLogic?.ProcessInput(combatInput);
        _cameraRecoil.Update(Time.deltaTime);
    }

    private void SubscribeCurrentWeapon()
    {
        if (_currentWeaponLogic == null || _currentWeaponView == null) return;

        _currentWeaponLogic.OnShot += HandleShot;
        _currentWeaponLogic.OnHit += HandleHit;
        _currentWeaponLogic.OnReloadStarted += _currentWeaponView.PlayReloadAnimation;
    }

    private void UnsubscribeCurrentWeapon()
    {
        if (_currentWeaponLogic == null || _currentWeaponView == null) return;

        _currentWeaponLogic.OnShot -= HandleShot;
        _currentWeaponLogic.OnHit -= HandleHit;
        _currentWeaponLogic.OnReloadStarted -= _currentWeaponView.PlayReloadAnimation;
    }

    private void HandleShot()
    {
        _currentWeaponView.PlayShootEffects();
        _cameraRecoil.GenerateRecoil();
    }

    private void HandleHit(HitscanHitInfo hitInfo)
    {
        _effectsService.HandleHit(hitInfo);
    }
}