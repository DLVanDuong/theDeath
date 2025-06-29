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
        Debug.Log("Enemy is dead");
        enemy.animator.SetTrigger("Die");
        enemy.agent.isStopped = true; // Stop the NavMeshAgent

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
