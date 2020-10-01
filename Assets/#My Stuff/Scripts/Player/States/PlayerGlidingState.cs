public class PlayerGlidingState : IState
{
    Player Owner;

    public PlayerGlidingState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter() { }
    public void Execute() { }
    public void Exit() { }

    public void CheckStateChanges()
    {
        if (Owner.HasStoppedReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFallingState(Owner));
        }
    }
}