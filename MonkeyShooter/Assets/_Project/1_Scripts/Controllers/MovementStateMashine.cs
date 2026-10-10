using UnityEngine;

public class MovementStateMashine
{
    [SerializeField] private MovementConfig _config;
    [SerializeField] private CharacterMover _mover;

    private MovementStateMachine _stateMachine;

    public MovementStateMashine(MovementConfig config,  CharacterMover mover)
    {
        _config = config;
        _mover = mover;
    }

    public void InitializeStateMachine()
    {
        _stateMachine = new MovementStateMachine();

        var airborneState = new AirborneState(_mover, _config, _stateMachine);
        var groundedState = new GroundedState(_mover, _config, _stateMachine, airborneState);
        var wallLatchState = new WallLatchState(_mover, _config, _stateMachine);

        airborneState.SetGroundedState(groundedState);
        airborneState.SetWallLatchState(wallLatchState);
        wallLatchState.SetStates(airborneState, groundedState);

        _stateMachine.Initialize(groundedState);
    }

    public void ProcessInput(in MovementFrameInput frameInput)
    {
        _stateMachine.Update(frameInput);
    }
}