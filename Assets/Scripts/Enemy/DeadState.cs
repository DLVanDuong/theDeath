using UnityEngine;

public class DeadState : IState
{
    private EnemyStateMachine enemy;

    public DeadState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }
    public void Enter()
    {
        if(enemy.enemyData.animationData != null)
        {
            enemy.animator.SetTrigger(enemy.enemyData.animationData.dieTrigger); // Kích hoạt Animator Dead
        }
        else
        {
            enemy.animator.SetTrigger("Dead"); // Kích hoạt Animator Dead nếu không có EnemyAnimationData
        }
        // Disable all colliders to prevent interaction with other objects
        var colliders = enemy.GetComponents<Collider>();
        if (colliders != null)
        {
            foreach (var collider in colliders)
            {
                collider.enabled = false; 
            }
        }
        GameObject.Destroy(enemy.gameObject, 3f); // Hủy sau 3 giây
    }
    public void Execute()
    {
        // Không làm gì trong trạng thái chết
    }
    public void Exit()
    {
        // Không cần làm gì khi rời khỏi trạng thái chết
    }
}
