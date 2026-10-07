using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "Configs/Weapon Config")]
public class WeaponData : ScriptableObject
{
    [field: Header("Visuals")]
    [field: SerializeField] public WeaponView WeaponViewPrefab { get; private set; }

    [field: Header("Damage & Range")]
    [field: SerializeField] public float BaseDamage { get; private set; } = 30f;
    [field: SerializeField] public float MinDamage { get; private set; } = 10f;
    [field: SerializeField] public float EffectiveDistance { get; private set; } = 15f; // До этой дистанции — BaseDamage
    [field: SerializeField] public float MaxDistance { get; private set; } = 50f;      // Дистанция сниженного/нулевого урона

    [field: Header("Fire Rate & Mode")]
    [field: SerializeField] public float FireRate { get; private set; } = 600f; // Выстрелов в минуту (RPM)
    [field: SerializeField] public bool IsAutomatic { get; private set; } = true;

    [field: Header("Ammo & Reload")]
    [field: SerializeField] public int MagazineSize { get; private set; } = 30;
    [field: SerializeField] public float ReloadTime { get; private set; } = 1.8f; // Время перезарядки в секундах

    [field: Header("Accuracy")]
    [field: SerializeField] public float SpreadAngle { get; private set; } = 1.5f; // Градус разброса

    // Вспомогательный расчет задержки между выстрелами в секундах
    public float FireDelay => 60f / FireRate;
}