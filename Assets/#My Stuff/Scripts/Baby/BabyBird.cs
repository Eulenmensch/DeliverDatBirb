using UnityEngine;
using UnityEngine.AI;

public class BabyBird : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] Quest quest;
    public Quest Quest
    {
        get => quest;
        set => quest = value;
    }

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
        Player = FindObjectOfType<Player>().gameObject;
        // Player = FindObjectOfType<PlayerController>().gameObject;
        BabyStateMachine = new StateMachine();
        BabyStateMachine.ChangeState(new BabyYardPatrollingState(this));
    }

    private void Update()
    {
        BabyStateMachine.UpdateState();
    }
}