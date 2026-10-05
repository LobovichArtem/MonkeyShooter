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


        if (_mover.VerticalVelocity <= 0f && _mover.IsTouchingWall)
        {
            if (_currentLatchCount < _config.WallLatchCount)
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
            Vector3 localDir = _mover.transform.InverseTransformDirection(input.MoveDirection);
            localDir.z *= _config.AirForwardMultiplier;
            localDir.x *= _config.AirStrafeMultiplier;

            Vector3 wishDir = _mover.transform.TransformDirection(localDir).normalized;

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