using UnityEngine;

[CreateAssetMenu(fileName = "WeaponKickConfig", menuName = "Configs/WeaponKickConfig")]
public class WeaponKickConfigSO : ScriptableObject
{
    [field: Header("Recoil Vectors")]
    [field: SerializeField] public Vector3 KickPosition { get; private set; } = new Vector3(0f, 0.02f, -0.1f);  // Z - назад, Y - подскок
    [field: SerializeField] public Vector3 KickRotation { get; private set; } = new Vector3(-8f, 1f, 2f);      // X - задирание дула вверх

    [field: Header("Animation Curves")]
    [field: SerializeField] public AnimationCurve PositionCurve { get; private set; } = AnimationCurve.Linear(0, 0, 1, 0);
    [field: SerializeField] public AnimationCurve RotationCurve { get; private set; } = AnimationCurve.Linear(0, 0, 1, 0);

    [field: Header("Settings")]
    [field: SerializeField] public float Duration { get; private set; } = 0.15f;
    [field: SerializeField] public float Snappiness { get; private set; } = 20f;
    [field: SerializeField] public float ReturnSpeed { get; private set; } = 15f;
}