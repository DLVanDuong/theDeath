using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    // Biến lưu trữ sat thương của vũ khí , từ bên ngoài vào
    private float damage;

    //ponent lưu trữ collider của vũ khí hitbox
    private Collider hitboxCollider;

    //Danh sách để lưu trữ các đối tượng đã bị trúng đòn
    private List<Collider> hitObjects = new List<Collider>();

    private void Awake()
    {
        // lấy component collider của đối tượng này
        hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider == null)
        {
            Debug.LogError("eaponHitbox yêu cầu một component Collider trên cùng một GameObject!");
            return;
        }
        // đảm bảo hitbox luôn tắt khi bắt đầu
        hitboxCollider.enabled = false;
    }
    public void SetDamage(float damageValue)
    {
        // Set giá trị sát thương từ bên ngoài
        this.damage = damageValue;
    }
    private void OnTriggerEnter(Collider other)
    {
        // Bỏ qua nếu đã đánh trúng trong lần vung này
        if (hitObjects.Contains(other))
        {
            return;
        }
        hitObjects.Add(other);

        if (other.CompareTag("Enemy"))
        {
            // lấy component stats ca đối tượng bị trúng đòn
            // Lưu ý: chúng ta ép kiểu int vì hàm takedamage thường nhận giá trị số nguyên (int)
            CharacterHealth enemyHealth = other.GetComponent<CharacterHealth>();
            if (enemyHealth != null)
            {
                // Gọi hàm TakeDamage trên đối tượng bị trúng đòn
                enemyHealth.TakeDamage((int)damage);
                Debug.Log("Đã đánh trúng " + other.name + " và gây " + (int)this.damage + " sát thương.");
            }
            else
            {
                Debug.LogWarning("Đối tượng Enemy không có component CharacterHealth!");
            }
        }
    }
    public void EnableHitbox()
    {
        hitObjects.Clear(); // Xóa danh sách các đối tượng đã trúng đòn

        if(hitboxCollider != null)
        {

            hitboxCollider.enabled = true; // Bật hitbox để nhận va chạm
        }
    }
    public void DisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false; // Tắt hitbox để không nhận va chạm
        }
    }
}
