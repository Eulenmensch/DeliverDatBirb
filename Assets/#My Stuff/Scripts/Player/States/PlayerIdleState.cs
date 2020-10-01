public class PlayerIdleState : IState
{
    Player Owner;

    public PlayerIdleState(Player _owner)
    {
        this.Owner = _owner;
    }

    public void Enter() { }
    public void Execute() { }
    public void Exit() { }
}