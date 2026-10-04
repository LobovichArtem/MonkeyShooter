public interface IMovementState
{
    void Enter();
    void Exit();
    void Update(in MovementFrameInput input);
}