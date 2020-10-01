using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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

    public Vector2 MovementInput { get; private set; }
    public bool IsReceivingJumpInput { get; private set; }
    public bool HasStoppedReceivingJumpInput { get; private set; }
    public bool IsReceivingFlapInput { get; private set; }

    public Vector3 MoveVector { get; set; }
    public Vector3 VelocityGravitational { get; set; }
    public bool Grounded { get; private set; }
    public bool InDialogue { get; private set; }

    public StateMachine PlayerStateMachine { get; private set; }
    public CharacterController CharacterControllerRef { get; private set; }


    private void Start()
    {
        PlayerStateMachine = new StateMachine();
        PlayerStateMachine.ChangeState(new PlayerIdleState(this));

        CharacterControllerRef = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerStateMachine.UpdateState();
        Grounded = IsGrounded();

        Move();
        SetRotationToMoveDirection();
    }

    void SetRotationToMoveDirection()
    {
        Vector3 lookDirection = new Vector3(MovementInput.x, 0.0f, MovementInput.y);
        if (lookDirection.magnitude >= 0.1)
        {
            transform.DORotateQuaternion(Quaternion.LookRotation(lookDirection, Vector3.up), 0.4f).SetEase(Ease.OutCirc);
        }
    }

    void Move()
    {
        CharacterControllerRef.Move(MoveVector * Time.deltaTime);
    }

    bool IsGrounded()
    {
        return (CharacterControllerRef.collisionFlags & CollisionFlags.Below) != 0;
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        float cameraAngle = (Camera.transform.rotation.eulerAngles.y) * Mathf.Deg2Rad;
        Vector2 inputVector = context.ReadValue<Vector2>();
        Vector2 rotatedInputVector = new Vector2(
            inputVector.x * Mathf.Cos(-cameraAngle) - inputVector.y * Mathf.Sin(-cameraAngle),
            inputVector.x * Mathf.Sin(-cameraAngle) + inputVector.y * Mathf.Cos(-cameraAngle)
        );
        MovementInput = rotatedInputVector;
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsReceivingJumpInput = true;
        }
        else if (context.canceled)
        {
            IsReceivingJumpInput = false;
            HasStoppedReceivingJumpInput = true;
        }
    }
}