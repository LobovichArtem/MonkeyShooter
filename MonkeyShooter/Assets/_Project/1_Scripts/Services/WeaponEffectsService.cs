using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParticleType
{
    [field: SerializeField] public ParticleKey ParticleKey { get; private set; }
    [field: SerializeField] public PhysicsMaterial PhysicsMaterial { get; private set; }
    [field: SerializeField] public int PrewarmCount { get; private set; } = 5;
}

public class WeaponEffectsService : MonoBehaviour
{
    [SerializeField] private ParticleType[] _particleTypes;
    [SerializeField] private int _defaultCapacity = 15;
    [SerializeField] private int _maxSize = 100;

    private readonly Dictionary<ParticleKey, ParticlePool> _particlePools = new();
    private ParticlePrefabsLibrary _particlePrefabsLibrary;

    private void Start()
    {
        _particlePrefabsLibrary = ServiceLocator.Get<ParticlePrefabsLibrary>();

        InitializePools();
        ServiceLocator.Register(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<WeaponEffectsService>();

        foreach (var pool in _particlePools.Values)
        {
            pool?.Dispose();
        }
        _particlePools.Clear();
    }

    private void InitializePools()
    {
        foreach (var type in _particleTypes)
        {
            if (string.IsNullOrEmpty(type.ParticleKey.Key))
                continue;

            var prefab = _particlePrefabsLibrary.GetPrefab(type.ParticleKey.Key);
            if (prefab != null)
            {
                var particleSystemPrefab = prefab.GetComponent<ParticleSystem>();
                if (particleSystemPrefab != null)
                {
                    var pool = new ParticlePool(particleSystemPrefab, _defaultCapacity, _maxSize, transform);
                    for (int i = 0; i < type.PrewarmCount; i++)
                    {
                        var ps = pool.Get();
                        pool.Release(ps);
                    }
                    _particlePools[type.ParticleKey] = pool;
                }
            }
        }
    }

    public void HandleHit(HitscanHitInfo hitInfo)
    {
        if (hitInfo.Collider == null) 
            return;

        ParticleKey key = null;
        var mat = hitInfo.Collider.sharedMaterial;

        foreach (var type in _particleTypes)
        {
            if (type.PhysicsMaterial == mat)
            {
                key = type.ParticleKey;
                break;
            }
        }

        if (string.IsNullOrEmpty(key.Key) == false && _particlePools.TryGetValue(key, out var pool))
        {
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitInfo.Normal);
            pool.Get(hitInfo.Point, rot, hitInfo.Collider.transform);
            return;
        }

        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = hitInfo.Point;
        marker.transform.localScale = Vector3.one * 0.2f;
        if (marker.TryGetComponent<Collider>(out var collider))
            Destroy(collider);
        Destroy(marker, 5);
    }
}