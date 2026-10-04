public class MovementStateMachine
{
    public IMovementState CurrentState { get; private set; }

    public void Initialize(IMovementState initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }

    public void ChangeState(IMovementState newState)
    {
        if (newState == null || newState == CurrentState)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void Update(in MovementFrameInput input)
    {
        CurrentState?.Update(input);
    }
}