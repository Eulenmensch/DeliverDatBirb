using UnityEngine;
using System.Collections;

public class BabyYardIdleState : IState
{
    BabyBird Owner;

    public BabyYardIdleState(BabyBird _owner)
    {
        this.Owner = _owner;
    }

    public void Enter()
    {
        float idleTime = Random.Range(0.0f, Owner.MaxIdleTime);
        Owner.StartCoroutine(Idle(idleTime));
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        //When the birds go to this state from flocking, they are still excited so this is where the particles get shut off
        Owner.ExcitedParticles.Stop();
    }

    private IEnumerator Idle(float _idleTime)
    {
        yield return new WaitForSeconds(_idleTime);
        Owner.BabyStateMachine.ChangeState(new BabyYardPatrollingState(Owner));
    }
}