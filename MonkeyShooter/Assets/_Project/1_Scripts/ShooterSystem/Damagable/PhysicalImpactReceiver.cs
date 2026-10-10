using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalImpactReceiver : MonoBehaviour, IKnockbackable
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void AddImpulse(Vector3 force, Vector3 hitPoint)
    {
        if (_rigidbody == null || _rigidbody.isKinematic) return;

        _rigidbody.AddForceAtPosition(force, hitPoint, ForceMode.Impulse);
    }
}