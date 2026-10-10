using System;
using UnityEngine;

public class ParticlePool : GenericPool<ParticleSystem>
{
    private readonly ParticleSystem _prefab;
    private readonly Transform _poolRoot;

    public ParticlePool(ParticleSystem prefab, int defaultCapacity = 15, int maxSize = 100, Transform poolRoot = null)
        : base(defaultCapacity, maxSize, false)
    {
        _prefab = prefab;
        _poolRoot = poolRoot;
    }

    public ParticleSystem Get(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var ps = Get();
        var trans = ps.transform;

        trans.position = position;
        trans.rotation = rotation;
        if (parent != null)
            trans.SetParent(parent, false);

        return ps;
    }

    protected override ParticleSystem CreateInstance()
    {
        var particle = GameObject.Instantiate(_prefab, _poolRoot);
        var autoReturn = particle.gameObject.AddComponent<ParticleAutoReturn>();
        autoReturn.Initialize(this);
        return particle;
    }

    protected override void OnGet(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
        //if (!ps.isPlaying) ps.Play();
    }

    protected override void OnRelease(ParticleSystem ps)
    {
        //ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.gameObject.SetActive(false);
    }

    protected override void OnDestroyInstance(ParticleSystem ps)
    {
        if (ps != null)
            GameObject.Destroy(ps.gameObject);
    }
}