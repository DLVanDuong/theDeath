// PlayerMoveState.cs
public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState() { }

    public override void UpdateState()
    {
        // Cập nhật hướng xoay và animation của nhân vật
        Ctx.HandleRotation();
        Ctx.HandleAnimation();

        // Kiểm tra để chuyển state
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void CheckSwitchStates()
    {
        // Nếu người chơi nhấn nút tấn công, chuyển sang AttackState
        if (Ctx.IsAttackPressed)
        {
            SwitchState(Factory.Attack());
        }
        // Nếu người chơi ngừng di chuyển, quay về IdleState
        else if (Ctx.CurrentMoveInput.x == 0 && Ctx.CurrentMoveInput.y == 0)
        {
            SwitchState(Factory.Idle());
        }
    }
}