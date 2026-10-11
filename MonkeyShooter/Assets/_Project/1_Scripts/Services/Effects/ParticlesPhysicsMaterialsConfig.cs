using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticlesPhysicsMaterialsConfig", menuName = "ParticleManagment/ParticlesPhysicsMaterialsConfig")]
public class ParticlesPhysicsMaterialsConfig : ScriptableObject
{
    [Serializable]
    public class ParticleType
    {
        [field: SerializeField] public ParticleKey ParticleKey { get; private set; }
        [field: SerializeField] public PhysicsMaterial PhysicsMaterial { get; private set; }
        [field: SerializeField] public int PrewarmCount { get; private set; } = 5;
    }

    [field: SerializeField] private ParticleType[] _particleTypes;
    [field: SerializeField] private int _defaultCapacity = 15;
    [field: SerializeField] private int _maxSize = 100;

    public ParticleType[] ParticleTypes => _particleTypes;
    public int DefaultCapacity => _defaultCapacity;
    public int MaxSize => _maxSize;
}