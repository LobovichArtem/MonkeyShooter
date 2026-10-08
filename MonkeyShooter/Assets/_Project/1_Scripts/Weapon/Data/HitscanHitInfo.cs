using UnityEngine;

public readonly struct HitscanHitInfo
{
    public Vector3 Point { get; }
    public Vector3 Normal { get; } //под вопросом
    public Collider Collider { get; }   //под вопросом

    public HitscanHitInfo(Vector3 point, Vector3 normal, Collider collider = null)
    {
        Point = point;
        Normal = normal;
        Collider = collider;
    }
}