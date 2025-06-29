using UnityEngine;

public class IdleState : IState
{
    private EnemyStateMachine enemy;
    private float timer;
    private float waitTime = 4f; // Thời gian đứng yên trước khi đi tuần.

    public IdleState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }
    public void Enter()
    {
        Debug.Log("Enemy Enter Idle State");
        
        enemy.agent.isStopped = true; // Dừng NavMeshAgent khi vào trạng thái Idle
        enemy.animator.ResetTrigger("Attack"); // Reset trigger Attack nếu có
        enemy.animator.SetFloat("Speed", 0f); // Đặt tốc độ về 0 trong Animator

        waitTime = Random.Range(2f, 5f); // Tùy chỉnh thời gian chờ ngẫu nhiên giữa 2-5 giây
        int idleIndex = Random.Range(0, 2);
        enemy.animator.SetInteger("IdleIndex", idleIndex); // Chọn ngẫu nhiên Idle Animation

        //kích hoạt Animator Idle
        enemy.animator.SetTrigger("PlayIdleAction");

        timer = 0f; // Reset timer khi vào trạng thái Idle
    }
    public void Execute()
    { 
        // nếu thấy Player, chuyển sang trạng thái Chase
        if(Vector3.Distance(enemy.transform.position, enemy.player.position) < enemy.sightRange)
        {
            enemy.ChangeState(new ChaseState(enemy));
            return;
        } 

        timer += Time.deltaTime; // Tăng timer theo thời gian thực
      
        //hết thời gian chờ, chuyển sang trạng thái Patrol
        if (timer >= waitTime)
        {
            enemy.ChangeState(new PatrolState(enemy));
        }
    }
    public void Exit()
    {
        Debug.Log("Enemy Exit Idle State");
        enemy.agent.isStopped = false; // Dừng agent khi rời khỏi trạng thái Idle
        // Không cần làm gì đặc biệt khi rời khỏi trạng thái Idle
        enemy.animator.ResetTrigger("PlayIdleAction");
       
    }
}
