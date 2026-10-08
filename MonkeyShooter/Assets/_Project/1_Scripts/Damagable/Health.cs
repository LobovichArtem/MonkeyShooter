using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [field: SerializeField] public float MaxHealth { get; private set; } = 100f;
    [field: SerializeField] public float CurrentHealth { get; private set; }

    public event Action<float, Vector3, Vector3> OnDamageTaken;
    public event Action OnDied;

    public bool IsDead => CurrentHealth <= 0f;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (IsDead) return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - damage);
        OnDamageTaken?.Invoke(damage, hitPoint, hitNormal);

        if (IsDead)
        {
            OnDied?.Invoke();
            Destroy(gameObject);
        }
    }
}