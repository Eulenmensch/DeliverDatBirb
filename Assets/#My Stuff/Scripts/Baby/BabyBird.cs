using UnityEngine;
using UnityEngine.AI;

public class BabyBird : MonoBehaviour
{
    [Header("General")]
    public ParticleSystem ExcitedParticles;

    [Header("Patrolling")]
    public Transform[] PatrolPoints;
    public float DefaultSpeed;
    public Vector2 MinMaxPatrolTime;
    public float MaxIdleTime;

    [Header("Flocking")]
    public float FlockSpeed;

    public GameObject Player { get; private set; }
    public StateMachine BabyStateMachine { get; private set; }

    private void Start()
    {
        // Player = FindObjectOfType<Player>().gameObject; //FIXME: Use this once the player script is useable and delete the next line
        Player = FindObjectOfType<PlayerController>().gameObject;
        BabyStateMachine = new StateMachine();
        BabyStateMachine.ChangeState(new BabyYardPatrollingState(this));
    }

    private void Update()
    {
        BabyStateMachine.UpdateState();
    }
}