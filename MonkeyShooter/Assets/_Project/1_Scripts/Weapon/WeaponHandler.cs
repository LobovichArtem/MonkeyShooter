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

    public WeaponHandler(Transform cameraTransform, Transform weaponContainer)
    {
        _cameraTransform = cameraTransform ?? throw new ArgumentNullException(nameof(cameraTransform));
        _weaponContainer = weaponContainer ?? throw new ArgumentNullException(nameof(weaponContainer));
        _effectsService = ServiceLocator.Get<WeaponEffectsService>();
    }

    public void EquipWeapon(WeaponData newWeaponData)
    {
        if (newWeaponData == null) return;

        // 1. Отписываемся и уничтожаем старый визуал
        UnsubscribeCurrentWeapon();

        if (_currentWeaponView != null)
        {
            UnityEngine.Object.Destroy(_currentWeaponView.gameObject);
        }

        // 2. Создаем чистую логику
        _currentWeaponLogic = new HitscanWeapon(newWeaponData);

        // 3. Спавним новый визуал и привязываем к контейнеру у камеры
        if (newWeaponData.WeaponViewPrefab != null)
        {
            _currentWeaponView = UnityEngine.Object.Instantiate(newWeaponData.WeaponViewPrefab, _weaponContainer);
            _currentWeaponView.transform.localPosition = Vector3.zero;
            _currentWeaponView.transform.localRotation = Quaternion.identity;
        }

        // 4. Подписываем визуал на события логики
        SubscribeCurrentWeapon();

        // Уведомляем внешние сервисы (например, WeaponEffectsService) о смене ствола
        OnWeaponChanged?.Invoke(_currentWeaponLogic);
    }

    public void ProcessInput(in CombatFrameInput combatInput)
    {
        Transform shootOrigin = _cameraTransform;

        _currentWeaponLogic?.ProcessInput(combatInput, shootOrigin);
    }

    private void SubscribeCurrentWeapon()
    {
        if (_currentWeaponLogic == null || _currentWeaponView == null) return;

        _currentWeaponLogic.OnShot += HandleShot;
        _currentWeaponLogic.OnReloadStarted += _currentWeaponView.PlayReloadAnimation;
    }

    private void UnsubscribeCurrentWeapon()
    {
        if (_currentWeaponLogic == null || _currentWeaponView == null) return;

        _currentWeaponLogic.OnShot -= HandleShot;
        _currentWeaponLogic.OnReloadStarted -= _currentWeaponView.PlayReloadAnimation;
    }

    private void HandleShot(HitscanHitInfo hitInfo)
    {
        _currentWeaponView.PlayShootEffects();
        _effectsService.HandleShot(hitInfo);
    }
}