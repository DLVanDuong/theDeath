using Unity.VisualScripting;
using UnityEngine;

public class PatrolState : IState
{
    private EnemyStateMachine enemy;
    
    public PatrolState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }
    public void Enter()
    { 
        Vector3 lookTarget = enemy.patrolPoint[enemy.currentPoint].position;
        lookTarget.y = enemy.transform.position.y; 
        enemy.transform.LookAt(lookTarget); 

// Reset trigger Idle nếu có
        enemy.animator.ResetTrigger("PlayIdleAction"); 

// Bật NavMeshAgent khi vào trạng thái Patrol   
        enemy.agent.isStopped = false;     

        enemy.agent.SetDestination(enemy.patrolPoint[enemy.currentPoint].position);

// Cập nhật tốc độ trong Animator
        enemy.agent.speed = enemy.walkSpeed;

    }
    public void Execute()
    { 
        // Nếu thấy Player, chuyển sang trạng thái Chase
        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < enemy.sightRange) 
        {
            enemy.ChangeState(new ChaseState(enemy));
            return;// Tránh thực hiện các lệnh bên dưới nếu đã chuyển trạng thái
        }
        // Kiểm tra khoảng cách đến điểm tuần tra hiện tại
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance < 0.5f)
        {
            //chuyển sang điểm tuần tra khác
            enemy.currentPoint = (enemy.currentPoint + 1) % enemy.patrolPoint.Length;
            enemy.ChangeState(new IdleState(enemy)); // Chuyển sang trạng thái Idle sau khi đến điểm tuần tra
        }
       
    }
    public void Exit()
    {
        Debug.Log("Enemy Exit Patrol State / Kẻ địch rời khỏi trạng thái tuần tra");
        
    }
}
