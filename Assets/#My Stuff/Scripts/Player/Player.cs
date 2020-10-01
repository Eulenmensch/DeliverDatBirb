using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject cameraRef;
    [SerializeField] Animator playerAnimator;

    [SerializeField] float speed;
    [SerializeField] float airControlFactor;
    [SerializeField] float gravitationalAcceleration;
    [SerializeField] float jumpHeight;
    [SerializeField] float flapHeight;
    [SerializeField] float fallModifier;
    [SerializeField] float shortJumpModifier;
    [SerializeField] float glideModifier;

    #region properties for the serializeFields
    public GameObject Camera { get { return cameraRef; } set { cameraRef = value; } }
    public Animator PlayerAnimator { get { return playerAnimator; } set { playerAnimator = value; } }

    public float Speed { get { return speed; } set { speed = value; } }
    public float AirControlFactor { get { return airControlFactor; } set { airControlFactor = value; } }
    public float GravitationalAcceleration { get { return gravitationalAcceleration; } set { gravitationalAcceleration = value; } }
    public float JumpHeight { get { return jumpHeight; } set { jumpHeight = value; } }
    public float FlapHeight { get { return flapHeight; } set { flapHeight = value; } }
    public float FallModifier { get { return fallModifier; } set { fallModifier = value; } }
    public float ShortJumpModifier { get { return shortJumpModifier; } set { shortJumpModifier = value; } }
    public float GlideModifier { get { return glideModifier; } set { glideModifier = value; } }
    #endregion

    public float HorizontalInput { get; private set; }
    public float VerticalInput { get; private set; }
    public bool IsReceivingJumpInput { get; private set; }
    public bool HasStoppedReceivingJumpInput { get; private set; }
    public bool IsReceivingFlapInput { get; private set; }

    public Vector3 MoveDirection { get; private set; }
    public Vector3 VelocityGravitational { get; private set; }

    public StateMachine PlayerStateMachine { get; private set; }

    private CharacterController CharacterControllerRef;


    private void Start()
    {
        PlayerStateMachine = new StateMachine();

        PlayerStateMachine.ChangeState(new PlayerIdleState(this));
    }

    private void Update()
    {
        PlayerStateMachine.UpdateState();
    }
}