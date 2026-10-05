using UnityEngine;

public class WallLatchState : IMovementState
{
    private readonly CharacterMover _mover;
    private readonly MovementConfig _config;
    private readonly MovementStateMachine _stateMachine;

    private AirborneState _airborneState;
    private GroundedState _groundedState;

    private float _latchTimer;

    public WallLatchState(
        CharacterMover mover,
        MovementConfig config,
        MovementStateMachine stateMachine)
    {
        _mover = mover;
        _config = config;
        _stateMachine = stateMachine;
    }

    public void SetStates(AirborneState airborneState, GroundedState groundedState)
    {
        _airborneState = airborneState;
        _groundedState = groundedState;
    }

    public void Enter()
    {
        _latchTimer = _config.WallLatchDuration;

        // ПОЛНАЯ ЗАМОРОЗКА ДВИЖЕНИЯ ПРИ ЗАЦЕПЛЕНИИ
        _mover.SetHorizontalVelocity(Vector3.zero);
        _mover.SetVerticalVelocity(0);
        _mover.SetGravityMultiplier(_config.WallLatchGravitatyMultiplier);
    }

    public void Exit()
    {
        _mover.ResetGravityMultiplier();
    }

    public void Update(in MovementFrameInput input)
    {
        // 1. Приземление на землю
        if (_mover.IsGrounded)
        {
            _stateMachine.ChangeState(_groundedState);
            return;
        }

        // 2. Каждый кадр зависания сбрасываем гравитацию и движение
        _mover.SetHorizontalVelocity(Vector3.zero);

        // 3. Отскок при нажатии прыжка
        if (input.IsJumpRequested)
        {
            ExecuteWallJump(input.LookDirection);
            _stateMachine.ChangeState(_airborneState);
            return;
        }

        // 4. Отсчет таймера
        _latchTimer -= Time.deltaTime;

        if (_latchTimer <= 0f)
        {
            _stateMachine.ChangeState(_airborneState);
            return;
        }
    }

    private void ExecuteWallJump(Vector3 lookDirection)
    {
        lookDirection.y = 0f;
        lookDirection.Normalize();

        Vector3 jumpDirection = lookDirection.normalized;

        // 1. Устанавливаем четкую горизонтальную скорость от стены
        _mover.SetHorizontalVelocity(jumpDirection * _config.BaseMoveSpeed);
        _mover.SetVerticalVelocity(_config.JumpForce);
    }
}