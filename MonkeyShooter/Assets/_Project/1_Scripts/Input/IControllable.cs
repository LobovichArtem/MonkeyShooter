using UnityEngine;

public interface IControllable
{
    string RequiredActionMap { get; }
    bool IsActive { get; }
    void EnableControl(InputService input);
    void DisableControl();
}