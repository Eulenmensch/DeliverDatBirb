public class PlayerJumpingState : IState
{
    Player Owner;

    public PlayerJumpingState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter() { }
    public void Execute() { }
    public void Exit() { }

    private void CheckStateChanges()
    {
        if (Owner.IsReceivingFlapInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFlappingState(Owner));
        }
        if (Owner.HasStoppedReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFallingState(Owner));
        }
        else
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerGlidingState(Owner));
        }
    }
}