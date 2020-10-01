public class PlayerFallingState : IState
{
    Player Owner;

    public PlayerFallingState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter() { }

    public void Execute()
    {
        CheckStateChanges();
    }

    public void Exit() { }

    public void CheckStateChanges()
    {
        if (Owner.IsReceivingFlapInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFlappingState(Owner));
        }
        if (Owner.Grounded)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerIdleState(Owner));
        }
    }
}