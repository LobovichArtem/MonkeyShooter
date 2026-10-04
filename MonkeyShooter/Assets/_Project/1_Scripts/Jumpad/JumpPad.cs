// Висит на объекте JumpPad
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private JumpPadConfig _config;

    private void OnTriggerEnter(Collider other)
    {
        // other - это объект, который ВОШЕЛ в триггер джампада (т.е. Игрок)
        if (other.TryGetComponent<IImpulseReceiver>(out var receiver))
        {
            Vector3 impulse = transform.TransformDirection(_config.ImpulseDirection) * _config.ImpulseForce;
            receiver.AddImpulse(impulse);
        }
    }
}