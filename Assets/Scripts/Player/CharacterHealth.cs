using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    [Tooltip("Gán file Scriptable Object Character chứa chỉ số gốc của kẻ địch này vào đây.")]
    [SerializeField] private Character characterData;

    public float currentHealth { get; private set; }

    private void Awake()
    {
        if (characterData != null)
        {
            currentHealth = characterData.maxhealth;
        }
        else
        {
            Debug.LogError("Chưa gán Character Data cho " + gameObject.name);
            currentHealth = 100; // Giá trị mặc định nếu quên gán
        }        
    }
    public void TakeDamage(int damage)
    {
       // Có thể thêm logic tính toán giáp ở đây, ví dụ:
        // float finalDamage = damage - characterData.defense;
        // if (finalDamage < 1) finalDamage = 1;

        currentHealth -= damage;
        Debug.Log(transform.name + " nhận " + damage + " sát thương. Máu còn lại: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    private void Die()
    {
        Debug.Log(transform.name + " đã chết.");
        // Thêm logic xử lý khi chết, ví dụ: hủy đối tượng, phát hiệu ứng, v.v.
        Destroy(gameObject);
    }
}
