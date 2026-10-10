using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ParticlesLibrary", menuName = "ParticleManagment/Particles Library")]
public class ParticlesLibrary : ScriptableObject
{
    [Serializable]
    public struct ParticleData
    {
        [field: SerializeField] public string Key { get; private set; }
        [field: SerializeField] public GameObject Prefab { get; private set; }
    }

    [field: SerializeField] private List<ParticleData> _particles = new();

    private Dictionary<string, GameObject> _cache;

    public GameObject GetPrefab(string key)
    {
        if (_cache == null)
        {
            _cache = new Dictionary<string, GameObject>();
            foreach (var item in _particles)
            {
                if (!string.IsNullOrEmpty(item.Key) && item.Prefab != null)
                {
                    _cache[item.Key] = item.Prefab;
                }
            }
        }

        if (_cache.TryGetValue(key, out var prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"[ParticleClipLibrary] Prefab not found for key: {key}");
        return null;
    }

    public static IEnumerable<string> GetAllKeys()
    {
        var libraries = Resources.LoadAll<ParticlesLibrary>("");
        foreach (var lib in libraries)
        {
            foreach (var item in lib._particles)
            {
                if (!string.IsNullOrEmpty(item.Key))
                {
                    yield return item.Key;
                }
            }
        }
    }
}