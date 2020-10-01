public class StateMachine
{
    public IState CurrentState { get; private set; }

    public void ChangeState(IState _newState)
    {
        if (CurrentState != null)
        {
            CurrentState.Exit();
        }
        CurrentState = _newState;
        CurrentState.Enter();
    }

    public void UpdateState()
    {
        if (CurrentState != null)
        {
            CurrentState.Execute();
        }
    }
}
