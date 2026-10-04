using System;

public abstract class BaseInput<T, P> : IControllable
    where T : IInputData
    where P : InputPacker<T>, new()
{
    public abstract string RequiredActionMap { get; }
    public bool IsActive => InputService != null;

    public InputService InputService { get; private set; }
    protected readonly P _packer = new();

    public event Action OnEnableControl;
    public event Action OnDisableControl;

    public BaseInput() => DisableControl();

    public virtual void EnableControl(InputService input)
    {
        InputService = input;

        var map = InputService.GetMap(RequiredActionMap);
        if (map != null)
        {
            _packer.Initialize(map);
        }
        OnEnableControl?.Invoke();

    }

    public virtual void DisableControl()
    {
        InputService = null;
        OnDisableControl?.Invoke();
    }

    //public T GetInput()
    //{
    //    return _packer.Pack(InputService.CurrentMap);
    //}

    private T _cachedInput;
    private int _lastUpdateFrame = -1;

    public T GetInput()
    {
        if (InputService == null) 
            return default;

        return _packer.Pack();

    }

}