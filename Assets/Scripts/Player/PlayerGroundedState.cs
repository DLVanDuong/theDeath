using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine stateMachine, PlayerStateFactory playerStateFactory)
        : base(stateMachine, playerStateFactory) { }
    
    public override void EnterState() { }
    
    public override void UpdateState()
    {       
        CheckSwitchStates();
    }
    public override void ExitState() { }
    public override void CheckSwitchStates()
    {
        //N?u ng??i ch?i ?ang nh?y, chuy?n sang tr?ng thái nh?y
        if (Ctx.isJumpPressed)
        {
            SwitchState(Factory.Jump());
            return;
        }
        //N?u ng??i ch?i ?ang t?n công, chuy?n sang tr?ng thái t?n công
        if(Ctx.IsAttackPressed)
        {
            SwitchState(Factory.Attack());
            return;
        }
        // N?u ng??i ch?i ?ang né ?òn ho?c l?n, chuy?n sang tr?ng thái né ?òn
        if (Ctx.isDodgePressed)
        {
            SwitchState(Factory.Dodge());
            return;
        }
    }
}
