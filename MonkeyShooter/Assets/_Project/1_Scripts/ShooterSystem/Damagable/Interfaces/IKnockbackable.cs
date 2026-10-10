using UnityEngine;

public interface IKnockbackable
{
    void AddImpulse(Vector3 force, Vector3 hitPoint);
}