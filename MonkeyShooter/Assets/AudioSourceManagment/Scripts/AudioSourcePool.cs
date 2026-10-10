using UnityEngine;
using UnityEngine.Pool;
using System;
using Object = UnityEngine.Object;

public class AudioSourcePool : IDisposable
{
    private ObjectPool<AudioSource> _pool;
    private AudioSource _prefab;

    public AudioSourcePool(AudioSource prefab, int defaultCapacity = 10, int maxSize = 50, int startCount = 0)
    {
        _prefab = prefab ? prefab : throw new ArgumentNullException(nameof(prefab));

        _pool = new ObjectPool<AudioSource>(
            createFunc: CreateAudioSource,
            actionOnGet: source =>
            {
                source.gameObject.SetActive(true);
                if (source.TryGetComponent<AudioSourceAutoReturn>(out var autoReturn))
                {
                    autoReturn.BeginPlay();
                }
            },
            actionOnRelease: source => source.gameObject.SetActive(false),
            actionOnDestroy: source => Object.Destroy(source.gameObject),
            collectionCheck: false,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );

        if (startCount > 0)
        {
            Prewarm(startCount);
        }
    }

    private AudioSource CreateAudioSource()
    {
        var go = Object.Instantiate(_prefab, _prefab.transform.parent);
        go.playOnAwake = false;

        if (!go.TryGetComponent<AudioSourceAutoReturn>(out var autoReturn))
        {
            autoReturn = go.gameObject.AddComponent<AudioSourceAutoReturn>();
        }

        autoReturn.Initialize(source => _pool.Release(source));

        return go;
    }

    private void Prewarm(int count)
    {
        var buffer = new AudioSource[count];
        for (int i = 0; i < count; i++)
        {
            buffer[i] = _pool.Get();
        }
        for (int i = 0; i < count; i++)
        {
            _pool.Release(buffer[i]);
        }
    }

    public AudioSource Get()
    {
        return _pool.Get();
    }

    public void Release(AudioSource audioSource)
    {
        _pool.Release(audioSource);
    }

    public void Dispose()
    {
        _pool.Clear();
        _prefab = null;
    }
}