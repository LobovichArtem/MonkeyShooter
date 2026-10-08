using System;

public abstract class BaseInput<T, P> 
    where T : IInputData
    where P : InputPacker<T>, new()
{

    protected readonly P _packer = new();

    public event Action OnEnableControl;
    public event Action OnDisableControl;

    public BaseInput(P packer)
    {
        _packer = packer;
    }

    public virtual void EnableControl()
    {
        OnEnableControl?.Invoke();
    }

    public virtual void DisableControl()
    {
        OnDisableControl?.Invoke();
    }

    public T GetInput()
    {
        if (_packer == null) 
            return default;
        return _packer.Pack();
    }

}