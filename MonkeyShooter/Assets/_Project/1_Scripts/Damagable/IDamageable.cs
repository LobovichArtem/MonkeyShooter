using System;
using UnityEngine;

public interface IDamageable
{
    public event Action<float, Vector3, Vector3> OnDamageTaken;

    public void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitNormal);
}