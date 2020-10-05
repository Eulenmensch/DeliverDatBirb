using UnityEngine;
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
        // Debug.Log(CurrentState.ToString());
    }

    public void UpdateState()
    {
        if (CurrentState != null)
        {
            CurrentState.Execute();
        }
    }
}
