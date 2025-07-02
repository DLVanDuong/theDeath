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
        // Ta cần đợi animation tấn công chạy xong rồi mới chuyển state
        // normalizedTime > 1 nghĩa là animation đã chạy hết 1 vòng
        // Cần kiểm tra tên của state trong Animator để chắc chắn ta đang đợi đúng animation
        // "Attack" là tên của state tấn công trong Animator Layer của bạn
        if (Ctx.Animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            Ctx.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
        {
            CheckSwitchStates();
        }
        // Nếu bạn dùng Upper Body layer, index của layer có thể là 1
        // if (Ctx.Animator.GetCurrentAnimatorStateInfo(1).normalizedTime > 1.0f) ...
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