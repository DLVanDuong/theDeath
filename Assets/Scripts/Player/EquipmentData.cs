using UnityEngine;

public enum EquipmentSlot { Weapon,Shield,Armor}

[CreateAssetMenu(fileName = "New Equipment", menuName = "Equipment")]
public class EquipmentData : ScriptableObject
{
    public EquipmentSlot slot;

    [Tooltip("Vị trí này sẽ hiện lên nhân vật")]
    public GameObject equipPrefab;

    public AnimatorOverrideController overrideController;  
}
