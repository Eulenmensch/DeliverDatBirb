using UnityEngine;
using UnityEngine.AI;

public class BabyYardPatrollingState : IState
{
    BabyBird Owner;

    private NavMeshAgent BirdAgent;
    private Vector3 PlayerPosition;

    private float patrolTime;
    private float timer;

    public BabyYardPatrollingState(BabyBird _owner)
    {
        this.Owner = _owner;
    }

    public void Enter()
    {
        BirdAgent = Owner.GetComponent<NavMeshAgent>();
        BirdAgent.autoBraking = true;
        BirdAgent.speed = Owner.DefaultSpeed;

        patrolTime = Random.Range(Owner.MinMaxPatrolTime.x, Owner.MinMaxPatrolTime.y);

        GoToNextPoint();
    }

    public void Execute()
    {
        PlayerPosition = Owner.Player.transform.position;
        NavMeshHit hit; //samplepPosition needs an out
        if (NavMesh.SamplePosition(PlayerPosition, out hit, 1f, NavMesh.AllAreas))
        {
            Owner.BabyStateMachine.ChangeState(new BabyYardFlockState(Owner));
        }

        if (!BirdAgent.pathPending && BirdAgent.remainingDistance < BirdAgent.stoppingDistance + 0.5f)
        {
            GoToNextPoint();
        }

        timer += Time.deltaTime;
        if (timer >= patrolTime)
        {
            Owner.BabyStateMachine.ChangeState(new BabyYardIdleState(Owner));
        }
    }

    public void Exit()
    {

    }

    private void GoToNextPoint()
    {
        if (Owner.PatrolPoints.Length == 0) { return; }

        BirdAgent.destination = Owner.PatrolPoints[Random.Range(0, Owner.PatrolPoints.Length - 1)].position;
    }
}