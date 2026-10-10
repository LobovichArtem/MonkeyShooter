using UnityEngine;

public class ParticlePrefabsLibrary
{
    private readonly ParticlesLibrary[] _particleClipLibraries;
    public ParticlesLibrary[] ParticlePrefabsLibraries => _particleClipLibraries;
    public ParticlePrefabsLibrary()
    {
        _particleClipLibraries = Resources.LoadAll<ParticlesLibrary>("");

        if (_particleClipLibraries.Length == 0)
        {
            Debug.LogWarning("[ParticleSystemService] No ParticleClipLibrary assets found in Resources.");
        }
    }

    public GameObject GetPrefab(string key)
    {
        if (_particleClipLibraries == null) return null;

        foreach (var library in _particleClipLibraries)
        {
            var prefab = library.GetPrefab(key);
            if (prefab != null) return prefab;
        }

        Debug.LogWarning($"[ParticleSystemService] Particle prefab not found for key: {key}");
        return null;
    }
}