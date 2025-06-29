using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private EnemyData enemyData;
    private Animator animator;
    private CharacterController character;
    private NavMeshAgent agent;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    private int currentPatrolIndex = 0;
    private bool isPatrolling = true;
    private bool isChasing = false;
    private bool isAttacking = false;
    private float attackTimer = 0f;

    public Transform target;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        
        if (enemyData != null)
        {
            attackRange = enemyData.AttackRange;
        }
        
        StartPatrolling();
    }

    void Update()
    {
        if (target == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);

        if (distanceToPlayer <= detectionRange)
        {
            if (distanceToPlayer <= attackRange && !isAttacking)
            {
                StartAttack();
            }
            else if (!isAttacking)
            {
                ChasePlayer();
            }
        }
        else if (!isPatrolling)
        {
            ReturnToPatrol();
        }

        UpdateAttackCooldown();
        UpdateAnimation();
    }

    void StartPatrolling()
    {
        if (patrolPoints.Length == 0) return;
        
        isPatrolling = true;
        isChasing = false;
        agent.speed = patrolSpeed;
        GoToNextPatrolPoint();
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;
        
        currentPatrolIndex = Random.Range(0, patrolPoints.Length);
        agent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void ChasePlayer()
    {
        isPatrolling = false;
        isChasing = true;
        agent.speed = enemyData.Speed;
        agent.SetDestination(target.position);
    }

    void StartAttack()
    {
        isAttacking = true;
        isChasing = false;
        agent.isStopped = true;
        
        // Quay mặt về phía người chơi
        transform.LookAt(target);
        
        // Kích hoạt animation tấn công
        animator.SetTrigger("Attack");
        
        // Gây sát thương cho người chơi (cần thêm logic xử lý sát thương)
        
        attackTimer = enemyData.AttackCooldown;
    }

    void UpdateAttackCooldown()
    {
        if (isAttacking)
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                isAttacking = false;
                agent.isStopped = false;
            }
        }
    }

    void ReturnToPatrol()
    {
        isChasing = false;
        StartPatrolling();
    }

    void UpdateAnimation()
    {
        // Cập nhật animation dựa trên trạng thái
        animator.SetFloat("IsMoving", agent.velocity.magnitude);
        animator.SetBool("IsChasing", isChasing);
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị phạm vi phát hiện trong Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Hiển thị phạm vi tấn công
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Hiển thị đường đi tuần tra
        if (patrolPoints != null && patrolPoints.Length > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawWireSphere(patrolPoints[i].position, 0.5f);
                    if (i < patrolPoints.Length - 1 && patrolPoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                }
            }
            if (patrolPoints[0] != null && patrolPoints[patrolPoints.Length - 1] != null)
            {
                Gizmos.DrawLine(patrolPoints[patrolPoints.Length - 1].position, patrolPoints[0].position);
            }
        }
    }
}
