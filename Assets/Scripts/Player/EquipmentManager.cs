using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    // Dictionary để lưu trữ các vật phẩm đã trang bị theo vị trí
    private Dictionary<EquipmentSlot, EquipmentData> equippedItems = new Dictionary<EquipmentSlot, EquipmentData>();
    // Dictionary để lưu trữ các object đã spawn
    private Dictionary<EquipmentSlot, GameObject> spawnedObjects = new Dictionary<EquipmentSlot, GameObject>();

    [SerializeField] private Animator animator;
    [Tooltip("Điểm neo [transform] trên xương của nhân vật")]
    [SerializeField] private Transform weaponHoldPointR;
    [SerializeField] private Transform weaponHoldPointL;
    [SerializeField] private Transform shieldholdPoint;
    // có thểm thêm điểm neo ở đây

    private RuntimeAnimatorController controller;

    void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        // Lưu lại controller gốc khi bắt đầu game
        controller = animator.runtimeAnimatorController;
    }
    public void Equip(EquipmentData newItem)
    {
        
        // 1. Kiểm tra xem ở vị trí (slot) của vật phẩm mới có đang trang bị vật phẩm nào khác không.
        if (equippedItems.ContainsKey(newItem.slot))
        {
            // 2. Nếu có, gọi hàm Unequip để gỡ vật phẩm cũ ra trước.
            Unequip(newItem.slot);
        }

        // 3. Sau khi đã chắc chắn vị trí đó trống, tiến hành trang bị vật phẩm mới.
        equippedItems[newItem.slot] = newItem;

        // 4. Áp dụng AnimatorOverrideController từ vật phẩm (nếu có)
        if (newItem.overrideController != null)
        {
            animator.runtimeAnimatorController = newItem.overrideController;
        }

        // 5. Tạo Prefab của vật phẩm và gắn vào tay nhân vật
        if (newItem.equipPrefab != null)
        {
            Transform parentTransform = GetParentTransformForSlot(newItem.slot);
            if (parentTransform != null)
            {
                GameObject newObject = Instantiate(newItem.equipPrefab, parentTransform);
                newObject.transform.localPosition = Vector3.zero;
                newObject.transform.localRotation = Quaternion.identity;
                spawnedObjects[newItem.slot] = newObject;
            }
        }
        animator.SetBool("isEquipped", true);
    }
    public void Unequip(EquipmentSlot slot)
    {

        // Kiểm tra xem có vật phẩm nào ở vị trí này để gỡ không
        if (equippedItems.ContainsKey(slot))
        {
            EquipmentData oldItem = equippedItems[slot];
            // Xóa vật phẩm khỏi Dictionary dữ liệu
            equippedItems.Remove(slot);

            // Phá hủy GameObject của vật phẩm đã tạo ra
            if (animator.runtimeAnimatorController == oldItem.overrideController)
            {
                animator.runtimeAnimatorController = controller;
            }
            // Quay trở lại Animator Controller gốc
            animator.runtimeAnimatorController = controller;

            // Báo cho Animator biết là đã gỡ trang bị
            animator.SetBool("isEquipped", equippedItems.Count > 0);
        }
    }
    private Transform GetParentTransformForSlot(EquipmentSlot slot)
    {
        switch (slot)
        {
            case EquipmentSlot.RightHand:
                return weaponHoldPointR;
            case EquipmentSlot.lefHand:
                return weaponHoldPointL;
            case EquipmentSlot.Shield:
                return shieldholdPoint;           
            default:
                return null;
        }
    }
}
