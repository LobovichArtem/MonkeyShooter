using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    public Transform Camera;
    
    public WeaponData WeaponData;

    private HumanInput _humanInput;

    private void Start()
    {
        _humanInput = ServiceLocator.Get<InputService>().HumanInput as HumanInput;
        if (WeaponData != null)
            EquipWeapon();
    }

    [ContextMenu("Equip")]
    public void EquipWeapon()
    {
        GetComponent<HumanInputController>().SelectWeapon(WeaponData);
    }
    public void EquipWeapon(WeaponData weapon)
    {
        GetComponent<HumanInputController>().SelectWeapon(weapon);
    }

    private void Update()
    {
        if (_humanInput == null)
            return;

        var input = _humanInput.GetInput();
        if(input.IsInteract)
        {
            if(Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit))
            {
                if(hit.collider.TryGetComponent(out WeaponView weapon))
                {
                    EquipWeapon(weapon.WeaponData);
                }    
            }
        }
    }

}
