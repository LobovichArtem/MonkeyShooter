using UnityEngine;

[CreateAssetMenu(fileName = "JumpPadConfig", menuName = "Configs/JumpPadConfig")]
public class JumpPadConfig : ScriptableObject
{
    [field: SerializeField] public Vector3 ImpulseDirection { get; private set; } = Vector3.up;
    [field: SerializeField] public float ImpulseForce { get; private set; } = 25f;
    [field: SerializeField] public bool OverrideVerticalVelocity { get; private set; } = true;
}