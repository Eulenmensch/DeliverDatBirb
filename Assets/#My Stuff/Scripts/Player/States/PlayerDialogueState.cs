public class PlayerDialogueState : IState
{
    Player Owner;

    public PlayerDialogueState(Player _owner)
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
        if (!Owner.InDialogue)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerIdleState(Owner));
        }
    }
}