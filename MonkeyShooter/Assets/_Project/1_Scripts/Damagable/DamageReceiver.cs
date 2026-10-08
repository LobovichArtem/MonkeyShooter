using System;
using UnityEngine;

public class DamageReceiver : MonoBehaviour, IDamageable
{
    [field: SerializeField] public Health MainHealth { get; private set; }
    [field: SerializeField] public float DamageMultiplier { get; private set; } = 1.0f;

    public event Action<float, Vector3, Vector3> OnDamageTaken;

    private void Reset()
    {
        MainHealth = GetComponentInParent<Health>();
    }

    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        float finalDamage = damage * DamageMultiplier;
        OnDamageTaken?.Invoke(finalDamage, hitPoint, hitNormal);

        if (MainHealth != null)
        {
            MainHealth.TakeDamage(finalDamage, hitPoint, hitNormal);
        }
    }
}