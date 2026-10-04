using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InputPacker<T> where T : IInputData
{
    public abstract void Initialize(InputActionMap map);
    public abstract T Pack();
}

public class HumanPacker : InputPacker<HumanInputData>
{
    private InputAction _move;
    private InputAction _look;
    private InputAction _sprint;
    private InputAction _crouch;
    private InputAction _jump;
    private InputAction _scroll;
    private InputAction _action;
    private InputAction _altAction;
    private InputAction _interact;
    private InputAction _slotSelected;
    private InputAction _radio;
    private InputAction _measuring;
    private InputAction _inventory;

    public override void Initialize(InputActionMap map)
    {
        _move ??= map.FindAction("Move");
        _look ??= map.FindAction("Look");
        _jump ??= map.FindAction("Jump");
        _sprint ??= map.FindAction("Sprint");
        _crouch ??= map.FindAction("Crouch");
        _scroll ??= map.FindAction("Scroll");
        _action ??= map.FindAction("Action");
        _altAction ??= map.FindAction("AltAction");
        _interact ??= map.FindAction("Interact");
        _slotSelected ??= map.FindAction("SlotSelected");
        _radio ??= map.FindAction("Radio");
        _measuring ??= map.FindAction("Measuring");
        _inventory ??= map.FindAction("Inventory");
    }

    public override HumanInputData Pack()
    {      

        return new HumanInputData
        {
            Move = _move.ReadValue<Vector2>(),
            Look = _look.ReadValue<Vector2>(),
            IsSprint = _sprint.IsPressed(),
            IsJump = _jump.triggered,
            IsCrouch = _crouch.IsPressed(),
            Scroll = _scroll.ReadValue<Vector2>().y,
            IsAction = _action.triggered,
            IsAltAction = _altAction.triggered, 
            IsInteract = _interact.triggered,
            SlotSelected = _slotSelected.ReadValue<float>(),
            IsRadio = _radio.triggered,
            IsMeasuring = _measuring.triggered,
            IsInventory = _inventory.triggered
        };
    }
}

public class VehiclePacker : InputPacker<VehicleInputData>
{
    private InputAction _move;
    private InputAction _steering;
    private InputAction _look;
    private InputAction _interact;

    public override void Initialize(InputActionMap map)
    { 
        _move ??= map.FindAction("Move");
        _steering ??= map.FindAction("Steering");
        _look ??= map.FindAction("Look");
        _interact ??= map.FindAction("Interact");
    }
    public override VehicleInputData Pack()
    {
        return new VehicleInputData
        {
            Move = _move.ReadValue<float>(),
            Steering = _steering.ReadValue<float>(),
            Look = _look.ReadValue<Vector2>(),
            IsInteract = _interact.triggered
        };
    }
}


public class VehicleUpPacker : InputPacker<VehicleUpInputData>
{
    private InputAction _move;
    private InputAction _rotation;
    private InputAction _scroll;
    private InputAction _action;

    public override void Initialize(InputActionMap map)
    {
        _move ??= map.FindAction("Move");
        _rotation ??= map.FindAction("Rotation");
        _scroll ??= map.FindAction("Scroll");
        _action ??= map.FindAction("Action");
    }
    public override VehicleUpInputData Pack()
    {
        return new VehicleUpInputData
        {
            Move = _move.ReadValue<Vector2>(),
            Rotation = _rotation.ReadValue<float>(),
            Scroll = _scroll.ReadValue<Vector2>().y,
            IsAction = _action.triggered
        };
    }
}

public class OrbitCamPacker : InputPacker<OrbitInputData>
{
    private InputAction _move;
    private InputAction _look;
    private InputAction _action;
    private InputAction _altAction;
    private InputAction _scroll;
    private InputAction _height;
    private InputAction _isSprint;
    private InputAction _isFocus;

    public override void Initialize(InputActionMap map)
    {
        _move ??= map.FindAction("Move");
        _look ??= map.FindAction("Look");
        _action ??= map.FindAction("Action");
        _altAction ??= map.FindAction("AltAction");
        _scroll ??= map.FindAction("Scroll");
        _height ??= map.FindAction("Height");
        _isSprint ??= map.FindAction("Sprint");
        _isFocus ??= map.FindAction("Focus");
    }
    public override OrbitInputData Pack()
    {
        
        return new OrbitInputData
        {
            Move = _move.ReadValue<Vector2>(),
            Look = _look.ReadValue<Vector2>(),
            IsAction = _action.IsPressed(),
            IsAltAction = _altAction.IsPressed(),
            Scroll = _scroll.ReadValue<Vector2>().y,
            Height = _height.ReadValue<float>(),
            IsSprint = _isSprint.ReadValue<float>(),
            IsFocus = _isFocus.triggered
        };

    }
}