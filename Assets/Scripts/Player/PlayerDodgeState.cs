using UnityEngine;

public class PlayerDodgeState : PlayerGroundedState
{
    private readonly int DodgeHash = Animator.StringToHash("Dodge");
    private const float CrossFadeDuration = 0.1f;
    
    public PlayerDodgeState(PlayerStateMachine stateMachine, PlayerStateFactory playerStateFactory)
        : base(stateMachine, playerStateFactory) { }

    public override void EnterState()
    {
        base.EnterState();
        Ctx.isDodgePressed = false; 
        Ctx.Animator.CrossFadeInFixedTime(DodgeHash, CrossFadeDuration);
    }
    public override void UpdateState()
    {
       
    }
    public override void ExitState() { base.ExitState(); }

    public override void CheckSwitchStates()
    {
        SwitchState(Factory.Idle()); // Sau khi né xong, chuyển về trạng thái Idle
    }
}
