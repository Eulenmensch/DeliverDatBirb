public class PlayerFlappingState : IState
{
    Player Owner;

    public PlayerFlappingState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter() { }

    public void Execute()
    {
        CheckStateChanges();
    }

    public void Exit() { }

    private void CheckStateChanges()
    {
        if (Owner.IsReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerGlidingState(Owner));
        }
        if (Owner.HasStoppedReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFallingState(Owner));
        }
    }
}