using UnityEngine;

[CreateAssetMenu(fileName = "MovementConfig", menuName = "Configs/MovementConfig")]
public class MovementConfig : ScriptableObject
{
    [field: Header("Physics & Mass")]
    [field: SerializeField] public float BaseMass { get; private set; } = 1f;
    [field: SerializeField] public float GroundedStickForce { get; private set; } = -2f;
    [field: SerializeField] public float ImpulseDampingRate { get; private set; } = 5f;
    [field: SerializeField] public float MinMassThreshold { get; private set; } = 0.1f;

    [field: Header("Gravity Defaults")]
    [field: SerializeField] public float DefaultGravityMultiplier { get; private set; } = 1f;

    [field: Header("Speed & Jump")]
    [field: SerializeField] public float BaseMoveSpeed { get; private set; } = 8f;
    [field: SerializeField] public float JumpForce { get; private set; } = 7f;

    [field: Header("Air Physics & Momentum")]
    [field: SerializeField] public float AirAcceleration { get; private set; } = 15f;           // Постоянное подруливание в воздухе
    [field: SerializeField, Range(0f, 1f)] public float AirForwardMultiplier { get; private set; } = 1f;
    [field: SerializeField, Range(0f, 1f)] public float AirStrafeMultiplier { get; private set; } = 0.5f;
    [field: SerializeField] public float DirectionChangeImpulse { get; private set; } = 4f;     // Сила стрэйф-импульса
    [field: SerializeField] public float AirImpulseWindow { get; private set; } = 0.25f;       // Окно (в сек), в течение которого доступен импульс
    [field: Header("Jump Settings")]
    [field: SerializeField] public float JumpBufferTime { get; private set; } = 0.15f; // Окно буферизации (в секундах)
    [field: Header("Wall Latch Settings")]
    [field: SerializeField] public float WallLatchDuration { get; private set; } = 1.0f; // Время зависания на стене (в секундах)
}