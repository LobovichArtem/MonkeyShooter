
public interface IControllable
{
    string RequiredActionMap { get; }
    bool IsActive { get; }
    void EnableControl();
    void DisableControl();
}