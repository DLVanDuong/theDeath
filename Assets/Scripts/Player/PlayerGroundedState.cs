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
        //N?u ng??i ch?i ?ang nh?y, chuy?n sang tr?ng th�i nh?y
        if (Ctx.isJumpPressed)
        {
            SwitchState(Factory.Jump());
            return;
        }
        //N?u ng??i ch?i ?ang t?n c�ng, chuy?n sang tr?ng th�i t?n c�ng
        if(Ctx.IsAttackPressed)
        {
            SwitchState(Factory.Attack());
            return;
        }
        // N?u ng??i ch?i ?ang n� ?�n ho?c l?n, chuy?n sang tr?ng th�i n� ?�n
        if (Ctx.isDodgePressed)
        {
            SwitchState(Factory.Dodge());
            return;
        }
    }
}
