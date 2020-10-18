using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class BirdPatrolBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] PatrolPoints = null;
    [SerializeField] private GameObject Player = null;
    [SerializeField] private float FlockSpeed = 0;

    private float DefaultSpeed;
    private NavMeshAgent BirdAgent;

    private Vector3 PlayerPosition;

    private void Start()
    {
        BirdAgent = GetComponent<NavMeshAgent>();
        BirdAgent.autoBraking = true;

        DefaultSpeed = BirdAgent.speed;

        GoToNextPoint();
    }

    private void Update()
    {
        PlayerPosition = Player.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(PlayerPosition, out hit, 1f, NavMesh.AllAreas))
        {
            BirdAgent.speed = FlockSpeed;
            BirdAgent.destination = PlayerPosition;
        }

        else if (!BirdAgent.pathPending && BirdAgent.remainingDistance < BirdAgent.stoppingDistance + 0.5f)
        {
            BirdAgent.speed = DefaultSpeed;
            GoToNextPoint();
        }
    }

    private void GoToNextPoint()
    {
        if (PatrolPoints.Length == 0) { return; }

        BirdAgent.destination = PatrolPoints[Random.Range(0, PatrolPoints.Length - 1)].position;
    }
}
