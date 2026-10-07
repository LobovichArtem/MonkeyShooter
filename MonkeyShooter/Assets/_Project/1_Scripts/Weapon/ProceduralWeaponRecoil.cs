using System;
using UnityEngine;

public class ProceduralWeaponRecoil
{
    private readonly Transform _targetTransform;
    private WeaponKickConfigSO _config;

    private float _timer;
    private bool _isPlaying;

    private Vector3 _currentPosition;
    private Vector3 _targetPosition;

    private Vector3 _currentRotation;
    private Vector3 _targetRotation;

    public ProceduralWeaponRecoil(Transform targetTransform)
    {
        _targetTransform = targetTransform ?? throw new ArgumentNullException(nameof(targetTransform));
    }

    public void SetConfig(WeaponKickConfigSO config)
    {
        _config = config;
        ResetRecoil();
    }

    public void PlayRecoil()
    {
        if (_config == null) return;

        _timer = 0f;
        _isPlaying = true;
    }

    public void Update(float deltaTime)
    {
        if (_config == null) return;

        if (_isPlaying)
        {
            _timer += deltaTime;
            float normalizedTime = Mathf.Clamp01(_timer / _config.Duration);

            // Считываем значения из кривых
            float posEval = _config.PositionCurve.Evaluate(normalizedTime);
            float rotEval = _config.RotationCurve.Evaluate(normalizedTime);

            // Расчет целевых точек на текущий кадр
            _targetPosition = _config.KickPosition * posEval;
            _targetRotation = _config.KickRotation * rotEval;

            if (normalizedTime >= 1.0f)
            {
                _isPlaying = false;
            }
        }
        else
        {
            // Возврат целевых значений в 0, когда кривая закончилась
            _targetPosition = Vector3.Lerp(_targetPosition, Vector3.zero, _config.ReturnSpeed * deltaTime);
            _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, _config.ReturnSpeed * deltaTime);
        }

        // Плавное сглаживание трансформаций
        _currentPosition = Vector3.Lerp(_currentPosition, _targetPosition, _config.Snappiness * deltaTime);
        _currentRotation = Vector3.Lerp(_currentRotation, _targetRotation, _config.Snappiness * deltaTime);

        // Применение локальных трансформаций
        _targetTransform.localPosition = _currentPosition;
        _targetTransform.localRotation = Quaternion.Euler(_currentRotation);
    }

    private void ResetRecoil()
    {
        _timer = 0f;
        _isPlaying = false;
        _targetPosition = Vector3.zero;
        _currentPosition = Vector3.zero;
        _targetRotation = Vector3.zero;
        _currentRotation = Vector3.zero;

        if (_targetTransform != null)
        {
            _targetTransform.localPosition = Vector3.zero;
            _targetTransform.localRotation = Quaternion.identity;
        }
    }
}