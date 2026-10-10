using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceAutoReturn : MonoBehaviour
{
    [field: SerializeField] public AudioSource AudioSourceComponent { get; private set; }

    private Action<AudioSource> _onReturnAction;

    private void Awake()
    {
        AudioSourceComponent = GetComponent<AudioSource>();
    }

    public void Initialize(Action<AudioSource> onReturnAction)
    {
        _onReturnAction = onReturnAction;
        enabled = false;
    }

    public void BeginPlay()
    {
        enabled = true;
    }

    private void Update()
    {
        if (!AudioSourceComponent.isPlaying)
        {
            enabled = false;
            _onReturnAction?.Invoke(AudioSourceComponent);
        }
    }
}