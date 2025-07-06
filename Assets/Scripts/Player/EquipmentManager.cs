using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    private Dictionary<EquipmentSlot, EquipmentData> equippedItems = new Dictionary<EquipmentSlot, EquipmentData>();
    private Dictionary<EquipmentSlot, GameObject> spawnedObjects = new Dictionary<EquipmentSlot, GameObject>();

    // --- PHẦN NÂNG CẤP 1: Thêm biến để lưu trữ hitbox hiện tại ---
    private WeaponHitbox currentWeaponHitbox;

    [Header("Character Stats")]
    [SerializeField] private Character playerCharacterStats;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform weaponHoldPointR;
    [SerializeField] private Transform weaponHoldPointL;
    [SerializeField] private Transform shieldholdPoint;

    private RuntimeAnimatorController originalController;

    void Awake()
    {
        if (animator == null) { animator = GetComponent<Animator>(); }
        originalController = animator.runtimeAnimatorController;
    }

    public void Equip(EquipmentData newItem)
    {
        if (equippedItems.ContainsKey(newItem.slot)) { Unequip(newItem.slot); }

        equippedItems[newItem.slot] = newItem;

        if (newItem.overrideController != null) { animator.runtimeAnimatorController = newItem.overrideController; }

        if (newItem.equipPrefab != null)
        {
            Transform parentTransform = GetParentTransformForSlot(newItem.slot);
            if (parentTransform != null)
            {
                GameObject newObject = Instantiate(newItem.equipPrefab, parentTransform);
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localRotation = Quaternion.identity;
                spawnedObjects[newItem.slot] = newObject;

                // --- PHẦN NÂNG CẤP 2: Lấy và lưu lại hitbox của vũ khí mới ---
                currentWeaponHitbox = newObject.GetComponentInChildren<WeaponHitbox>();
                if (currentWeaponHitbox != null && playerCharacterStats != null)
                {
                    currentWeaponHitbox.SetDamage(playerCharacterStats.attack);
                }
            }
        }
        animator.SetBool("isEquipped", true);
    }

    public void Unequip(EquipmentSlot slot)
    {
        if (equippedItems.ContainsKey(slot))
        {
            // --- PHẦN SỬA LỖI QUAN TRỌNG: Phải phá hủy GameObject cũ ---
            if (spawnedObjects.ContainsKey(slot) && spawnedObjects[slot] != null)
            {
                Destroy(spawnedObjects[slot]);
                spawnedObjects.Remove(slot);
                // --- PHẦN NÂNG CẤP 3: Khi gỡ vũ khí, xóa tham chiếu hitbox ---
                if (slot == EquipmentSlot.RightHand) { currentWeaponHitbox = null; }
            }

            EquipmentData oldItem = equippedItems[slot];
            equippedItems.Remove(slot);

            if (animator.runtimeAnimatorController == oldItem.overrideController)
            {
                animator.runtimeAnimatorController = originalController;
            }
            animator.SetBool("isEquipped", equippedItems.Count > 0);
        }
    }

    // --- PHẦN NÂNG CẤP 4: Tạo các hàm public để PlayerStateMachine có thể gọi ---
    public void EnableCurrentWeaponHitbox()
    {
        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.EnableHitbox();
        }
    }

    public void DisableCurrentWeaponHitbox()
    {
        if (currentWeaponHitbox != null)
        {
            currentWeaponHitbox.DisableHitbox();
        }
    }

    private Transform GetParentTransformForSlot(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.RightHand: return weaponHoldPointR;
            case EquipmentSlot.lefHand: return weaponHoldPointL;
            case EquipmentSlot.Shield: return shieldholdPoint;
            default: return null;
        }
    }
}