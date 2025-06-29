using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class EnemyStateMachine : MonoBehaviour
{
    // === Trạng thái FSM ===
    [HideInInspector] public IState currentState;
    [HideInInspector] public bool isAttacking;

    [Header("AI Speed")]
    public float walkSpeed = 2f; // Tốc độ đi bộ của kẻ địch
    public float runSpeed = 4f; // Tốc độ chạy của kẻ địch
    // == Tham Chiếu Component ==
    [Header("Componets")]
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public Transform player;

    // == thông số AI ==
    [Header("AI Parameters")]
    public Transform[] patrolPoint;
    [HideInInspector] public int currentPoint = 0;
    public float sightRange = 10f;
    public float attackRange = 2f;

    [Header("Enemy Data")]
    public EnemyData enemyData;
    [HideInInspector] public int currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        // Lấy tham chiếu đến các component
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = enemyData.Speed; // Thiết lập tốc độ của NavMeshAgent từ dữ liệu EnemyData
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Khởi tạo dữ liệu Enemy
        
    }
    private void Start()
    {
        // Bắt đầu với trạng thái Patrol
        ChangeState(new IdleState(this));
        
        // Khởi tạo sức khỏe
        currentHealth = enemyData.Health;
    }
    void Update()
    {
        if(isDead) return;// Nếu đã chết, không làm gì cả
        UpDateMovementAnimator();
        // Luôn chạy hàm Execute của trạng thái hiện tại
        currentState?.Execute();
    }
    private void UpDateMovementAnimator()
    {
        // lấy tốc độ của NavMeshAgent
        float currentSpeed = agent.velocity.magnitude;

        // === THÊM DÒNG DEBUG NÀY VÀO ===
        Debug.Log("Current Agent Velocity Magnitude: " + currentSpeed);

        // cập nhật Animator với tốc độ hiện tại
        animator.SetFloat("Speed", currentSpeed);
    }
    public void ChangeState(IState newState)
    {
        //gọi hàm Exit của trạng thái hiện tại
        currentState?.Exit();
        // Chuyển sang trạng thái mới
        currentState = newState;
        // Gọi hàm Enter của trạng thái mới
        currentState?.Enter();
    }
    public void TakeDamage(int damage)
    {
        if(isDead) return; // Nếu đã chết, không nhận sát thương
        currentHealth -= damage;
        Debug.Log("Enemy HP: " + currentHealth);
        if(currentHealth <= 0)
        {
            isDead = true;
            ChangeState(new DeadState(this));
        }
    }
    public void OnAttackAnimationEnd()
    {
        isAttacking = false; // Reset trạng thái tấn công
        agent.isStopped = false; // Bật lại NavMeshAgent sau khi kết thúc tấn công
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            ChangeState(new AttackState(this)); // Trở lại trạng thái tấn công nếu vẫn trong tầm
        }
        else
        {
            ChangeState(new ChaseState(this)); // Chuyển sang trạng thái Chase nếu ra ngoài tầm
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
