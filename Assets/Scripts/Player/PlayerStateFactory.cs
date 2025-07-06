// PlayerStateFactory.cs
public class PlayerStateFactory
{
    private PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    // Các phương thức để tạo và trả về một state cụ thể
    public PlayerBaseState Idle()
    {
        return new PlayerIdleState(_context, this);
    }
    public PlayerBaseState Move()
    {
        return new PlayerMoveState(_context, this);
    }
    public PlayerBaseState Attack()
    {
        return new PlayerAttackState(_context, this);
    }
    // Bạn có thể thêm các state khác ở đây (ví dụ: Jump, Crouch...)
    public PlayerBaseState Jump()
    {
        return new PlayerAirborneState(_context, this);
    }
    public PlayerBaseState Dodge()
    {
        return new PlayerDodgeState(_context, this);
    }
    public PlayerBaseState Grounded()
    {
        return new PlayerGroundedState(_context, this);
    }
    public PlayerBaseState Airborne()
    {
        return new PlayerAirborneState(_context, this);
    }
}