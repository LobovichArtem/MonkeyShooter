using UnityEngine;

public interface IInputData { }

public struct HumanInputData : IInputData
{
    public Vector2 Move;
    public Vector2 Look;
    public bool IsJump;
    public bool IsSprint;
    public bool IsCrouch;
    public float Scroll;
    public bool IsAction;
    public bool IsActionHeld;
    public bool IsReloadPressed;
    public bool IsAltAction;
    public bool IsInteract;
    public float SlotSelected;
    public bool IsInventory;
}
