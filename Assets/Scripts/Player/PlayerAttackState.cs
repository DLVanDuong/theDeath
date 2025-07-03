// PlayerAttackState.cs
public class PlayerAttackState : PlayerBaseState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        // Kích hoạt animation tấn công
        Ctx.Animator.SetTrigger(Ctx.AttackHash);
        // Reset lại cờ nhấn nút ngay lập tức để tránh lặp lại tấn công
        Ctx.IsAttackPressed = false;
    }

    public override void UpdateState()
    {
        
    }

    public override void ExitState()
    {
        // Reset lại Trigger để đảm bảo an toàn cho lần kích hoạt tiếp theo
        Ctx.Animator.ResetTrigger(Ctx.AttackHash);
    }

    public override void CheckSwitchStates()
    {
        // Sau khi tấn công xong, kiểm tra xem người chơi có đang di chuyển không
        // để quyết định quay về Idle hay Move
        if (Ctx.CurrentMoveInput.x != 0 || Ctx.CurrentMoveInput.y != 0)
        {
            SwitchState(Factory.Move());
        }
        else
        {
            SwitchState(Factory.Idle());
        }
    }
}