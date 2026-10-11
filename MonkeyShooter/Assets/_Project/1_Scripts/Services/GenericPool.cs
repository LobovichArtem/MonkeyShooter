using System;
using UnityEngine.Pool;

public abstract class GenericPool<T> : IDisposable where T : class
{
    private readonly ObjectPool<T> _pool;

    protected GenericPool(int defaultCapacity = 15, int maxSize = 100, bool collectionCheck = false)
    {
        _pool = new ObjectPool<T>(
            CreateInstance,
            OnGet,
            OnRelease,
            OnDestroyInstance,
            collectionCheck,
            defaultCapacity,
            maxSize);
    }

    public T Get() => _pool.Get();
    public void Release(T element) => _pool.Release(element);
    public void Prewarm(int count)
    {
        var items = new T[count];
        for (int i = 0; i < count; i++)
        {
            items[i] = _pool.Get();
        }
        for (int i = 0; i < count; i++)
        {
            _pool.Release(items[i]);
        }
    }
    protected abstract T CreateInstance();
    protected virtual void OnGet(T element) { }
    protected virtual void OnRelease(T element) { }
    protected virtual void OnDestroyInstance(T element) { }

    public virtual void Dispose() => _pool.Clear();
}