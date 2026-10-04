using UnityEngine;

public class AirborneState : IMovementState
{
    private readonly CharacterMover _mover;
    private readonly MovementConfig _config;
    private readonly MovementStateMachine _stateMachine;
    private GroundedState _groundedState;
    private WallLatchState _wallLatchState;

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

    public void Enter() { Debug.Log("Airborne"); }
    public void Exit() { Debug.Log("AirborneExit"); }

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
            _stateMachine.ChangeState(_wallLatchState);
            return;
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

    //private void ExecuteWallJump(Vector3 wallBounceDirection)
    //{
    //    Vector3 lookForward = _mover.transform.forward;
    //    lookForward.y = 0f;
    //    lookForward.Normalize();

    //    // Складываем отталкивание от стены и направление взгляда
    //    Vector3 jumpDirection = (wallBounceDirection + lookForward).normalized;

    //    // Выбираем максимум между базовой скоростью и текущей инерцией
    //    float currentSpeed = _mover.HorizontalVelocity.magnitude;
    //    float targetSpeed = Mathf.Max(_config.BaseMoveSpeed, currentSpeed);

    //    _mover.SetHorizontalVelocity(jumpDirection * targetSpeed);

    //    // Используем стандартную силу прыжка
    //    _mover.Jump(_config.JumpForce);
    //}
}