using UnityEngine;

[CreateAssetMenu(fileName = "RecoilConfig", menuName = "Configs/RecoilConfig")]
public class RecoilConfigSO : ScriptableObject
{
    [field: Header("Seed & Pre-generation")]
    [field: SerializeField] public int Seed { get; private set; } = 1337;
    [field: SerializeField] public int PreGeneratedShotsCount { get; private set; } = 100;

    [field: Header("Recoil Multipliers (Vector2: X - Pitch Up, Y - Yaw Side)")]
    [field: SerializeField] public Vector2 RecoilStepMin { get; private set; } = new Vector2(1.5f, -0.5f);
    [field: SerializeField] public Vector2 RecoilStepMax { get; private set; } = new Vector2(2.5f, 0.5f);

    [field: Header("Interpolation Settings")]
    [field: SerializeField] public float Snappiness { get; private set; } = 12f;
    [field: SerializeField] public float ReturnSpeed { get; private set; } = 6f;
}