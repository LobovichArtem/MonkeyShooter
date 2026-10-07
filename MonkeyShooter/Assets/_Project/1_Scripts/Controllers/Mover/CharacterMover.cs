using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMover : MonoBehaviour, IImpulseReceiver
{
    public CharacterController Controller { get; private set; }
    public MovementConfig Config { get; private set;  }

    private Vector3 _externalVelocity;
    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;

    private float _currentMass;
    private float _gravityMultiplier;

    // Храним данные последнего контакта со стеной
    private Vector3 _lastWallNormal;
    private Vector3 _lastWallContactPoint;
    private float _lastWallContactTime;

    public bool IsTouchingWall => Time.time - _lastWallContactTime < 0.1f; // Актуальность контакта (100 мс)

    public bool IsGrounded => Controller.isGrounded;
    public Vector3 HorizontalVelocity => _horizontalVelocity;
    public float VerticalVelocity => _verticalVelocity;


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Проверяем, что касаемся стены (угол нормали к Y близок к 0)
        if (Mathf.Abs(hit.normal.y) < 0.3f)
        {
            _lastWallNormal = hit.normal;
            _lastWallContactPoint = hit.point;
            _lastWallContactTime = Time.time;
        }
    }

    public void Initialize(MovementConfig config)
    {
        Config = config;
        _currentMass = Config.BaseMass;
        _gravityMultiplier = Config.DefaultGravityMultiplier;
        Controller = GetComponent<CharacterController>();
    }

    // Состояния напрямую задают или сглаживают целевую горизонтальную скорость
    public void SetHorizontalVelocity(Vector3 velocity)
    {
        _horizontalVelocity = velocity;
    }

    public void SetVerticalVelocity(float velocity)
    {
        _verticalVelocity = velocity;
    }

    public void SetGravityMultiplier(float multiplier)
    {
        _gravityMultiplier = multiplier;
    }

    public void ResetGravityMultiplier()
    {
        _gravityMultiplier = Config.DefaultGravityMultiplier;
    }

    public void Jump(float jumpForce)
    {
        if (IsGrounded)
        {
            _verticalVelocity = jumpForce;
        }
    }

    public void AddImpulse(Vector3 impulse)
    {
        float effectiveMass = Mathf.Max(_currentMass, Config.MinMassThreshold);
        Vector3 processedImpulse = impulse / effectiveMass;

        // Если падаем и получаем импульс вверх — гасим падение до 0
        if (processedImpulse.y > 0f && _verticalVelocity < 0f)
        {
            _verticalVelocity = 0f;
        }

        _externalVelocity += processedImpulse;
    }

    public bool TryGetWallBounceDirection(out Vector3 bounceDirection)
    {
        bounceDirection = Vector3.zero;

        if (!IsTouchingWall)
            return false;

        // Вектор от точки коллизии к центру контроллера
        Vector3 centerPos = transform.position + Vector3.up * (Controller.height * 0.5f);
        Vector3 directionFromHit = (centerPos - _lastWallContactPoint).normalized;
        directionFromHit.y = 0f;

        // Комбинируем нормаль стены и вектор от точки контакта
        bounceDirection = (_lastWallNormal + directionFromHit).normalized;
        return true;
    }



    private void FixedUpdate()
    {
        ApplyGravity();
        ProcessMovement();
    }

    private void ProcessMovement()
    {
        Vector3 finalVelocity = _horizontalVelocity;
        finalVelocity.y = _verticalVelocity;
        finalVelocity += _externalVelocity;

        Controller.Move(finalVelocity * Time.deltaTime);

        // Гашение внешнего импульса
        float effectiveMass = Mathf.Max(_currentMass, Config.MinMassThreshold);
        float damping = (Config.ImpulseDampingRate / effectiveMass) * Time.deltaTime;
        _externalVelocity = Vector3.Lerp(_externalVelocity, Vector3.zero, damping);
    }

    private void ApplyGravity()
    {
        if (IsGrounded && _verticalVelocity < 0)
        {
            _verticalVelocity = Config.GroundedStickForce;
        }

        float gravity = Physics.gravity.y * _gravityMultiplier;
        _verticalVelocity += gravity * Time.deltaTime;
    }
}