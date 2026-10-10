using UnityEngine;

public class PenetrationSurface : MonoBehaviour, IProtectable
{
    [field: Range(0f, 100f)]
    [field: SerializeField] public float ArmorValue { get; private set; } = 40f;
}