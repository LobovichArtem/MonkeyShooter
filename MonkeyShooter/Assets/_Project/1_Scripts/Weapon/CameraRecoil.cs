using System;
using UnityEngine;

public class CameraRecoil
{
    private readonly Transform _cameraTransform;

    private RecoilConfigSO _currentConfig;
    private Vector2[] _preGeneratedPattern;

    private int _currentIndex;

    private Vector2 _currentRotation;
    private Vector2 _targetRotation;

    private const float ThresholdToResetIndex = 0.01f;

    public CameraRecoil(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform ?? throw new ArgumentNullException(nameof(cameraTransform));
    }

    public void SetConfig(RecoilConfigSO config)
    {
        _currentConfig = config;
        _currentIndex = 0;
        _currentRotation = Vector2.zero;
        _targetRotation = Vector2.zero;

        if (_currentConfig != null)
        {
            GeneratePattern();
        }
    }

    public void GenerateRecoil()
    {
        if (_currentConfig == null || _preGeneratedPattern == null || _preGeneratedPattern.Length == 0)
            return;

        // Берем готовый шаг из предсгенерированного массива
        Vector2 recoilStep = _preGeneratedPattern[_currentIndex];

        // X — Pitch (подъем вверх), Y — Yaw (отклонение влево/вправо)
        _targetRotation += new Vector2(-recoilStep.x, recoilStep.y);

        // Увеличиваем индекс для следующего выстрела в очереди
        if (_currentIndex < _preGeneratedPattern.Length - 1)
        {
            _currentIndex++;
        }
    }

    public void Update(float deltaTime)
    {
        if (_currentConfig == null)
            return;

        // 1. Возврат целевого вращения в 0
        _targetRotation = Vector2.Lerp(_targetRotation, Vector2.zero, _currentConfig.ReturnSpeed * deltaTime);

        // 2. Сглаживание текущего вращения к целевому
        _currentRotation = Vector2.Lerp(_currentRotation, _targetRotation, _currentConfig.Snappiness * deltaTime);

        // 3. Применение поворота к локальному трансформу камеры
        _cameraTransform.localRotation = Quaternion.Euler(_currentRotation.x, _currentRotation.y, 0f);

        // 4. Сброс индекса очереди, когда отдача полностью затихла
        if (_currentIndex > 0 && _targetRotation.sqrMagnitude < ThresholdToResetIndex && _currentRotation.sqrMagnitude < ThresholdToResetIndex)
        {
            _currentIndex = 0;
        }
    }

    private void GeneratePattern()
    {
        int count = Mathf.Max(1, _currentConfig.PreGeneratedShotsCount);
        _preGeneratedPattern = new Vector2[count];

        System.Random random = new System.Random(_currentConfig.Seed);

        for (int i = 0; i < count; i++)
        {
            // Генерация случайного шага в заданном диапазоне
            float pitch = LerpRandom(random, _currentConfig.RecoilStepMin.x, _currentConfig.RecoilStepMax.x);
            float yaw = LerpRandom(random, _currentConfig.RecoilStepMin.y, _currentConfig.RecoilStepMax.y);

            _preGeneratedPattern[i] = new Vector2(pitch, yaw);
        }
    }

    private float LerpRandom(System.Random random, float min, float max)
    {
        return min + (float)random.NextDouble() * (max - min);
    }
}