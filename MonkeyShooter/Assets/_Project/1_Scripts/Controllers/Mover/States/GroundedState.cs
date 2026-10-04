using UnityEngine;

public class GroundedState : IMovementState
{
    private readonly CharacterMover _mover;
    private readonly MovementConfig _config;
    private readonly MovementStateMachine _stateMachine;
    private readonly AirborneState _airborneState;

    public GroundedState(
        CharacterMover mover,
        MovementConfig config,
        MovementStateMachine stateMachine,
        AirborneState airborneState)
    {
        _mover = mover;
        _config = config;
        _stateMachine = stateMachine;
        _airborneState = airborneState;
    }

    public void Enter() { }
    public void Exit() { }

    public void Update(in MovementFrameInput input)
    {
        // 1. Если потеряли землю (упали с уступа) — переходим в воздух
        if (!_mover.IsGrounded)
        {
            _stateMachine.ChangeState(_airborneState);
            return;
        }

        // 2. Прыжок
        if (input.IsJumpRequested)
        {
            _mover.Jump(_config.JumpForce);
            _stateMachine.ChangeState(_airborneState);
            return;
        }

        // 3. Расчет мгновенной земной скорости
        Vector3 targetVelocity = input.MoveDirection * _config.BaseMoveSpeed;
        _mover.SetHorizontalVelocity(targetVelocity);
    }
}