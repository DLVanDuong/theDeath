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
    private PlayerStateMachine playerControler;

    // == thông số AI ==
    [Header("AI Parameters")]
    public Transform[] patrolPoint;
    [HideInInspector] public int currentPoint = 0;
    public float sightRange = 10f;
    public float attackRange = 2f;
    [Range(0, 360)]
    public float viewAngle = 90;// góc nhìn
    public float hearingRange = 5; // nge chạy

    // == Trọng lực cho Enemy ==
    [Header("Gravity Settings")]
    [SerializeField] private float gravity = -9.81f; // Trọng lực
    [SerializeField] private float groundCheckDistance = 0.1f; // Khoảng cách kiểm tra mặt đất
    [SerializeField] private LayerMask groundLayer; // Lớp mặt đất để kiểm tra
    private Vector3 verticalVelocity; // Vận tốc thẳng đứng

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
        
        if(player!= null)
        {
            playerControler = player.GetComponent<PlayerStateMachine>();
        }      
    }
    public bool CanDetectPlayer()
    {
        //Kiểm tra xem có tham chiếu đến player không
        if (playerControler == null) return false;
        //kiểm tra tiếng động 
        bool isPlayerRunning = playerControler._isSprinting;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if(isPlayerRunning && distanceToPlayer <= hearingRange)
        {
            return true;
        }
        if(distanceToPlayer <= sightRange)
        {
            Vector3 derectionToPlayer =(player.position - transform.position).normalized;

            // kiểm tra góc nhìn
            if(Vector3.Angle(transform.forward, derectionToPlayer) < viewAngle / 2) 
            {
                float distanceToTarget = Vector3.Distance(transform.position,player.position);
                if (Physics.Raycast(transform.position, derectionToPlayer, distanceToTarget, LayerMask.GetMask("Default")))
                {
                    return false;
                }
                return true;
            }
        }
        // không thấy người chơi
        return false;
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
        if (isDead) return;// Nếu đã chết, không làm gì cả
        HandleGravity(); // Xử lý trọng lực cho Enemy
        if (agent.enabled)
        {
            UpDateMovementAnimator();
            // Luôn chạy hàm Execute của trạng thái hiện tại
            currentState?.Execute();
        }
    }
    private void UpDateMovementAnimator()
    {
        // lấy tốc độ của NavMeshAgent
        float currentSpeed = agent.velocity.magnitude;
        if (enemyData.animationData != null)
        {
            animator.SetFloat(enemyData.animationData.speedParam, currentSpeed);
        }
        else       
        {
            // cập nhật Animator với tốc độ hiện tại
            animator.SetFloat("Speed", currentSpeed);
        }
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
    private void HandleGravity()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if (isGrounded && verticalVelocity.y < 0)
        {
            // Nếu trên mặt đất, reset vận tốc rơi
            verticalVelocity.y = -2f; // Một gia tốc nhỏ để giữ kẻ địch trên mặt đất

            // Đảm bảo NaMeshAgent được bật để nó di chuyển trên mặt đất
            if(!agent.enabled)
            {
                agent.enabled = true;
            }
        }
        else
        {
            // Nếu không trên mặt đất, áp dụng trọng lực
            // Tắt NavMeshAgent để tránh va chạm không mong muốn
            if(agent.enabled)
            {
                agent.enabled = false; // Tắt NavMeshAgent khi không trên mặt đất
            }
            verticalVelocity.y += gravity * Time.deltaTime; // Cập nhật vận tốc rơi
        }
        // Di chuyển Enemy theo trục Y dựa trên vận tốc thẳng đứng
        // Chúng ta chỉ làm  điều này khi agent không được bật
        if(!agent.enabled)
        {
            transform.position += verticalVelocity * Time.deltaTime;
        }
    }
    private void OnDrawGizmosSelected()
    {
        // Tầm Nhìn
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        //Tầm tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        //Góc nhìn
        Vector3 fovLine1 = Quaternion.AngleAxis(viewAngle / 2, transform.up) * transform.forward * sightRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(viewAngle / 2, transform.up) * transform.forward * sightRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay (transform.position, fovLine2);

        //vẻ tầm nge
        Gizmos.color = new Color(1, 0, 1, 0.25f);
        Gizmos.DrawWireSphere (transform.position, hearingRange);
    }
}
