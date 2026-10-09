using UnityEngine;

public class AirborneState : IMovementState
{
    private readonly CharacterMover _mover;
    private readonly MovementConfig _config;
    private readonly MovementStateMachine _stateMachine;
    private GroundedState _groundedState;
    private WallLatchState _wallLatchState;

    private int _currentLatchCount;

    public AirborneState(
        CharacterMover mover,
        MovementConfig config,
        MovementStateMachine stateMachine)
    {
        _mover = mover;
        _config = config;
        _stateMachine = stateMachine;
    }

    public void SetGroundedState(GroundedState groundedState)
    {
        _groundedState = groundedState;
    }
    public void SetWallLatchState(WallLatchState wallLatchState)
    {
        _wallLatchState = wallLatchState;
    }

    public void Enter() { }
    public void Exit() {  }

    public void Update(in MovementFrameInput input)
    {
        // 1. Приземление
        if (_mover.IsGrounded && _mover.VerticalVelocity <= 0f)
        {
            _stateMachine.ChangeState(_groundedState);
            return;
        }


        if (_mover.IsTouchingWall)
        {
            if (_mover.VerticalVelocity <= 0f && _currentLatchCount < _config.WallLatchCount)
            {
                _currentLatchCount++;
                _stateMachine.ChangeState(_wallLatchState);
                return;
            }
        }

        // 3. Плавный Air Strafe
        Vector3 currentVelocity = _mover.HorizontalVelocity;

        if (input.MoveDirection != Vector3.zero)
        {
            // Вместо _mover.transform используем LookDirection (взгляд камеры по горизонтали)
            // Либо вычисляем локальные оси на основе input.LookDirection
            Vector3 lookForward = input.LookDirection;
            lookForward.y = 0f;
            lookForward.Normalize();
            Vector3 lookRight = Vector3.Cross(Vector3.up, lookForward);

            // Переводим мировой ввод в локальные оси камеры
            float forwardAmount = Vector3.Dot(input.MoveDirection, lookForward);
            float strafeAmount = Vector3.Dot(input.MoveDirection, lookRight);

            forwardAmount *= _config.AirForwardMultiplier;
            strafeAmount *= _config.AirStrafeMultiplier;

            // Собираем обратно желаемое направление движения с учетом множителей
            Vector3 wishDir = (lookForward * forwardAmount + lookRight * strafeAmount).normalized;

            float currentSpeed = currentVelocity.magnitude;
            Vector3 currentDir = currentSpeed > 0.001f ? currentVelocity / currentSpeed : wishDir;

            float dot = Vector3.Dot(currentDir, wishDir);
            float angleMultiplier = Mathf.Clamp01((1f + dot) * 0.5f + 0.2f);

            float targetSpeed = Mathf.Max(_config.BaseMoveSpeed, currentSpeed);
            Vector3 targetVelocity = wishDir * targetSpeed;

            float accelRate = _config.AirAcceleration * angleMultiplier;
            currentVelocity = Vector3.MoveTowards(
                currentVelocity,
                targetVelocity,
                accelRate * Time.deltaTime
            );
        }

        _mover.SetHorizontalVelocity(currentVelocity);
    }

    public void ResetLatchCount()
    {
        _currentLatchCount = 0;
    }
}