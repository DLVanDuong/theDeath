// PlayerIdleState.cs
public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        // Khi vào trạng thái Idle, đảm bảo các thông số animation di chuyển bằng 0
        Ctx.Animator.SetFloat(Ctx.HorizontalHash, 0);
        Ctx.Animator.SetFloat(Ctx.VerticalHash, 0);
    }

    public override void UpdateState()
    {
        // Mỗi frame, kiểm tra xem có nên chuyển state không
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
        // Nếu người chơi nhấn nút di chuyển, chuyển sang MoveState
        else if (Ctx.CurrentMoveInput.x != 0 || Ctx.CurrentMoveInput.y != 0)
        {
            SwitchState(Factory.Move());
        }
    }
}