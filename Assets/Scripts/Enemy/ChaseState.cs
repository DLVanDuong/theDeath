using UnityEngine;

public class ChaseState : IState
{
    private EnemyStateMachine enemy;

    public ChaseState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }
    public void Enter()
    {
        Debug.Log("Entering Chase State/ Đang chuyển sang trạng thái đuổi theo ");          
        // Đặt tốc độ chạy khi đuổi theo 
        enemy.agent.isStopped = false; // Bật NavMeshAgent

        enemy.agent.speed = enemy.runSpeed;


    }

    public void Execute()
    {
        // luôn theo player
        enemy.agent.SetDestination(enemy.player.position);

        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        //nếu người chơi ra ngoài tầm nhìn, chuyển sang trạng thái Patrol
        if (distanceToPlayer > enemy.sightRange)
        {
            enemy.ChangeState(new PatrolState(enemy));

        }
        else if (distanceToPlayer <= enemy.attackRange)
        {
            // Nếu người chơi trong tầm tấn công, chuyển sang trạng thái Attack
            enemy.ChangeState(new AttackState(enemy));
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Chase State/ Đang rời khỏi trạng thái đuổi theo");
        
    }
}
