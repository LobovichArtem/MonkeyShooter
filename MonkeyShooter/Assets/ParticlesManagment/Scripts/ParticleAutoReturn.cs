using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAutoReturn : MonoBehaviour
{
    private ParticlePool _pool;
    private ParticleSystem _system;

    public void Initialize(ParticlePool pool)
    {
        _pool = pool;
        _system = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        var ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        if (_pool != null)
        {
            _pool.Release(_system);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}