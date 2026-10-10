using UnityEngine;


[CreateAssetMenu(fileName = "GlobalWeaponSettings", menuName = "Configs/GlobalWeaponSettings")]
public class GlobalWeaponSettings : ScriptableObject
{
    private static GlobalWeaponSettings _instance;

    public static GlobalWeaponSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GlobalWeaponSettings>("GlobalWeaponSettings");
            }
            return _instance;
        }
    }

    [field: SerializeField] public LayerMask WeaponHitscanLayer { get; private set; }
    [field: SerializeField] public float PhysicsImpactMultiplier { get; private set; } = 10f;
}