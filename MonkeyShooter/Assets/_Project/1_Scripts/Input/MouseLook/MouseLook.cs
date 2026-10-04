using UnityEngine;

public sealed class MouseLook
{
    // Локальные поля (Backing fields)
    private float _sensitivityX = 5f;
    private float _sensitivityY = 5f;
    private float _smoothSpeed = 20f;
    private Vector2 _minMaxX = Vector2.zero;
    private Vector2 _minMaxY = Vector2.zero;

    // Свойства с приоритетом настроек из конфига
    public float SensitivityX => _mouseLookSettings != null ? _mouseLookSettings.Sensitivity : _sensitivityX;
    public float SensitivityY => _mouseLookSettings != null ? _mouseLookSettings.Sensitivity : _sensitivityY;
    public float SmoothSpeed => _mouseLookSettings != null ? _mouseLookSettings.SmoothSpeed : _smoothSpeed;

    public Vector2 MinMaxX => _mouseLookSettings != null ? _mouseLookSettings.MinMaxX : _minMaxX;
    public Vector2 MinMaxY => _mouseLookSettings != null ? _mouseLookSettings.MinMaxY : _minMaxY;

    // Динамическая проверка ограничений: если границы не Zero, значит Clamp активен
    private bool IsXRotationClamped => MinMaxX != Vector2.zero;
    private bool IsYRotationClamped => MinMaxY != Vector2.zero;

    private float _rotationX;
    private float _smoothRotationX;
    private float _rotationY;
    private float _smoothRotationY;

    public Transform Transform { get; private set; }
    private MouseLookSettigns _mouseLookSettings;

    // Конструктор 1: Ручная настройка
    public MouseLook(Transform head, float sensitivity, Vector2 minMaxX, Vector2 minMaxY, float smoothSpeed = 20)
    {
        Transform = head;
        _sensitivityX = _sensitivityY = sensitivity;
        _minMaxX = minMaxX;
        _minMaxY = minMaxY;
        _smoothSpeed = smoothSpeed;

        InitializeRotation();
    }

    // Конструктор 2: Через конфиг
    public MouseLook(Transform head, MouseLookSettigns settings)
    {
        Transform = head;
        _mouseLookSettings = settings;

        InitializeRotation();
    }

    private void InitializeRotation()
    {
        Vector3 angles = Transform.localEulerAngles;
        _rotationY = -NormalizeAngle(angles.x);
        _rotationX = NormalizeAngle(angles.y);

        _smoothRotationX = _rotationX;
        _smoothRotationY = _rotationY;
    }

    private float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }

    public void UpdateLook(float rotX, float rotY)
    {
        // Обработка X (Горизонталь)
        _rotationX += rotX * SensitivityX;
        if (IsXRotationClamped)
            _rotationX = Mathf.Clamp(_rotationX, MinMaxX.x, MinMaxX.y);
        else
            _rotationX = Mathf.Repeat(_rotationX, 360f);

        // Обработка Y (Вертикаль)
        _rotationY += rotY * SensitivityY;
        if (IsYRotationClamped)
            _rotationY = Mathf.Clamp(_rotationY, MinMaxY.x, MinMaxY.y);
        else
            _rotationY = Mathf.Repeat(_rotationY, 360f);

        // Плавное сглаживание
        _smoothRotationX = Mathf.LerpAngle(_smoothRotationX, _rotationX, SmoothSpeed * Time.smoothDeltaTime);
        _smoothRotationY = Mathf.LerpAngle(_smoothRotationY, _rotationY, SmoothSpeed * Time.smoothDeltaTime);

        Transform.localEulerAngles = new Vector3(-_smoothRotationY, _smoothRotationX, 0);
    }
}