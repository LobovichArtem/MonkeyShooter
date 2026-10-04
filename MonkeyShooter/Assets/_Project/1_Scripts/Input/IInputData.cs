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
    public bool IsAltAction;
    public bool IsInteract;
    public float SlotSelected;
    public bool IsRadio;
    public bool IsMeasuring;
    public bool IsInventory;
}

public struct VehicleInputData : IInputData
{
    public float Move;
    public float Steering;
    public Vector2 Look;
    public bool IsInteract;
    public bool Handbrake; // Jump или специальная кнопка
}

public struct OrbitInputData : IInputData
{
    public Vector2 Move;
    public Vector2 Look;
    public bool IsAction;
    public bool IsAltAction;
    public float Scroll;
    public float Height;
    public bool IsFocus;
    public float IsSprint;
}

public struct VehicleUpInputData : IInputData
{
    public Vector2 Move;
    public float Rotation;
    public float Scroll;
    public bool IsAction;
}