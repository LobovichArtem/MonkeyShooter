using UnityEngine;

public readonly struct HitscanHitInfo
{
    public bool DidHit { get; }
    public Vector3 OriginPoint { get; }
    public Vector3 HitPoint { get; }

    public HitscanHitInfo(bool didHit, Vector3 originPoint, Vector3 hitPoint)
    {
        DidHit = didHit;
        OriginPoint = originPoint;
        HitPoint = hitPoint;
    }
}