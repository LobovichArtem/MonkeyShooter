using System;
using UnityEngine;

[Serializable]
public struct ParticleKey
{
    [SerializeField] private string _key;
    public string Key => _key;

    public ParticleKey(string key)
    {
        _key = key;
    }

    public static implicit operator string(ParticleKey particleKey) => particleKey.Key;
    public static implicit operator ParticleKey(string key) => new ParticleKey(key);
}