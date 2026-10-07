using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    public WeaponData WeaponData;

    [ContextMenu("Equip")]
    public void EquipWeapon()
    {
        GetComponent<HumanInputController>().SelectWeapon(WeaponData);
    }
    
}
