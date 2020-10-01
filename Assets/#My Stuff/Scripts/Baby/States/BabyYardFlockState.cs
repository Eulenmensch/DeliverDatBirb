using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class BabyYardFlockState : IState
{
    BabyBird Owner;

    NavMeshAgent BirdAgent;
    private Vector3 PlayerPosition;


    public BabyYardFlockState(BabyBird _owner)
    {
        this.Owner = _owner;
    }

    public void Enter()
    {
        BirdAgent = Owner.GetComponent<NavMeshAgent>();
        BirdAgent.autoBraking = true;

        Owner.ExcitedParticles.Play();
    }
    public void Execute()
    {
        PlayerPosition = Owner.Player.transform.position;

        BirdAgent.speed = Owner.FlockSpeed;
        BirdAgent.destination = PlayerPosition;

        NavMeshHit hit; //samplePosition needs an out
        if (!NavMesh.SamplePosition(PlayerPosition, out hit, 1f, NavMesh.AllAreas))
        {
            Owner.BabyStateMachine.ChangeState(new BabyYardIdleState(Owner));
        }
    }
    public void Exit() { }
}