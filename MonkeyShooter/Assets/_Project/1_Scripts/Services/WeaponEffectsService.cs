
using System.Collections.Generic;
using UnityEngine;


public class WeaponEffectsService : MonoBehaviour
{
    [SerializeField] private ParticlesPhysicsMaterialsConfig _config;
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

    public void HandleHit(HitscanHitInfo hitInfo)
    {
        if (hitInfo.Collider == null)
            return;

        ParticleKey key = null;
        var mat = hitInfo.Collider.sharedMaterial;

        foreach (var type in _config.ParticleTypes)
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
            var p = pool.Get(hitInfo.Point, rot, hitInfo.Collider.transform);
            return;
        }

#if UNITY_EDITOR
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = hitInfo.Point;
        marker.transform.localScale = Vector3.one * 0.05f;
        if (marker.TryGetComponent<Collider>(out var collider))
            Destroy(collider);
        Destroy(marker, 5);
#endif
    }

    private void InitializePools()
    {
        foreach (var type in _config.ParticleTypes)
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
                    pool.Prewarm(type.PrewarmCount);
                    _particlePools[type.ParticleKey] = pool;
                }
            }
        }
    }


}