using UnityEngine;

public class TransformImpackReceiver : MonoBehaviour, IKnockbackable
{
    [SerializeField] private float _recoveryTime = 0.25f; // Время возврата в исходное положение
    [SerializeField] private float _maxHitPunchAngle = 35f; // Максимальный угол отклонения от удара
    [Header("Optional")]
    [SerializeField] private Transform _pivot;
    private Rigidbody _rigidbody;
    private Quaternion _initialLocalRotation;
    private Quaternion _targetLocalRotation;
    private float _recoveryTimer;
    private bool _isRecovering;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _initialLocalRotation = transform.localRotation;
        _targetLocalRotation = _initialLocalRotation;
        if (_pivot == null)
            _pivot = transform;
    }

    public void AddImpulse(Vector3 force, Vector3 hitPoint)
    {
        if (_rigidbody != null && !_rigidbody.isKinematic)
        {
            // Если включен полноценный рагдолл — работаем через физику
            _rigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
        }
        else
        {
            // Персонаж жив (аниматор активен): делаем процедурный хит-реакций рывок
            ApplyProceduralHitReaction(force, hitPoint);
        }
    }

    private void ApplyProceduralHitReaction(Vector3 force, Vector3 hitPoint)
    {
        // 1. Вычисляем направление удара
        Vector3 hitDirection = force.normalized;

        // 2. Используем точку попадания (hitPoint) и центр трансформа для определения плеча силы и оси вращения
        // Если hitPoint передан нулевым (на всякий случай), берем позицию кости
        Vector3 leverArm = hitPoint - _pivot.position;

        // Ось вращения зависит от того, куда пришелся удар относительно центра кости
        Vector3 rotationAxis = Vector3.Cross(leverArm.normalized, hitDirection);
        if (rotationAxis == Vector3.zero)
        {
            rotationAxis = Vector3.Cross(hitDirection, transform.forward);
        }
        if (rotationAxis == Vector3.zero)
        {
            rotationAxis = transform.right;
        }
        rotationAxis.Normalize();

        // 3. Вычисляем угол на основе силы удара и ограничиваем его максимальным значением _maxHitPunchAngle
        float rawAngle = force.magnitude; // Коэффициент масштабирования силы (можно настроить)
        float angle = Mathf.Clamp(rawAngle, 0f, _maxHitPunchAngle);
        
        Quaternion punchRotation = Quaternion.AngleAxis(angle, _pivot.InverseTransformDirection(rotationAxis));

        _initialLocalRotation = transform.localRotation;
        _targetLocalRotation = punchRotation * _initialLocalRotation;

        transform.localRotation = _targetLocalRotation;

        _recoveryTimer = _recoveryTime;
        _isRecovering = true;
    }

    private void LateUpdate()
    {
        if (!_isRecovering)
            return;

        if (_recoveryTimer > 0f)
        {
            _recoveryTimer -= Time.deltaTime;
            float t = 1f - Mathf.Clamp01(_recoveryTimer / _recoveryTime);

            transform.localRotation = Quaternion.Slerp(_targetLocalRotation, _initialLocalRotation, t);
            
            if (_recoveryTimer <= 0f)
            {
                _isRecovering = false;
                transform.localRotation = _initialLocalRotation;
            }
        }
    }
}