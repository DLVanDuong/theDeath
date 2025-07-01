using UnityEngine;

public class AttackState : IState
{
    private EnemyStateMachine enemy;
    private float time;
    private bool isAttacking = false; // Biến để theo dõi trạng thái tấn công


    public AttackState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Entering Attack State/ Đang chuyển sang trạng thái tấn công");
        enemy.agent.isStopped = true; // Dừng NavMeshAgent
        enemy.agent.velocity = Vector3.zero; // Đặt vận tốc về 0 để dừng lại hoàn toàn
        enemy.animator.applyRootMotion = false;

        // Nhìn về phía player trước khi tấn công
        

        // Bắt đầu tấn công
        StartAttack();
        
        time = 0f; // Reset thời gian tấn công
    }

    private void StartAttack()
    {
        isAttacking = true;
        if(enemy.enemyData.animationData != null)
        {
            //enemy.animator.SetInteger(enemy.enemyData.animationData.attackTrigger, Random.Range(0, 2)); // Chọn ngẫu nhiên Attack Animation
            enemy.animator.SetTrigger(enemy.enemyData.animationData.attackTrigger); // Kích hoạt Animator Attack    
        }
        else
        {
            enemy.animator.SetTrigger("Attack"); // Kích hoạt Animator Attack nếu không có EnemyAnimationData
        }
    }

    public void Execute()
    {
        time += Time.deltaTime;

        // Chỉ nhìn theo player khi không đang trong animation tấn công
        
            Vector3 lookTarget = enemy.player.position;
            lookTarget.y = enemy.transform.position.y;
            enemy.transform.LookAt(lookTarget);
        
        // Nếu người chơi ra ngoài tầm tấn công và không đang trong animation tấn công
        if (Vector3.Distance(enemy.transform.position, enemy.player.position) > enemy.attackRange && !isAttacking)
        {
            enemy.ChangeState(new ChaseState(enemy));
            return;
        }

        // Nếu thời gian tấn công đã đủ và không đang trong animation tấn công
        if (time >= enemy.enemyData.AttackCooldown && !isAttacking)
        {
            StartAttack();
            time = 0f; // Reset thời gian tấn công
            
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Attack State/ Đang rời khỏi trạng thái tấn công");
        enemy.agent.isStopped = false; // Bật lại NavMeshAgent
        isAttacking = false;
    }
}
