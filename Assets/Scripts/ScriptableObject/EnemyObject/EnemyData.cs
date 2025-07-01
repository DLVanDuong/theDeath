using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    new public string name = "New Enemy";
    public int Health = 100; // Sức khỏe của kẻ địch
    public int Damage = 10; // Sát thương của kẻ địch
    public float Speed = 2f; // Tốc độ di chuyển của kẻ địch
    public float AttackRange = 1f; // Khoảng cách tấn công của kẻ địch
    public float AttackCooldown = 1f; // Thời gian hồi chiêu giữa các đòn tấn công

    [Header("Animation Data")]
    public EnemyAnimationData animationData;
}
