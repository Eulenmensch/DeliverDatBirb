using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

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
    public GameObject CameraRef { get { return cameraRef; } set { cameraRef = value; } }
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

    public Vector2 MovementInput { get; private set; } = Vector2.zero;
    public bool IsReceivingJumpInput { get; private set; } = false;
    public bool HasStoppedReceivingJumpInput { get; set; } = true;
    public bool IsReceivingFlapInput { get; private set; } = false;

    public Vector3 MoveVector { get; set; } = Vector3.zero;
    public Vector3 VelocityGravitational { get; set; } = Vector3.zero;
    public bool Grounded { get; private set; } = false;
    public bool InDialogue { get; private set; } = false;

    public StateMachine PlayerStateMachine { get; private set; }
    public CharacterController CharacterControllerRef { get; private set; }


    private void Start()
    {
        PlayerStateMachine = new StateMachine();
        PlayerStateMachine.ChangeState(new PlayerIdleState(this));

        CharacterControllerRef = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        Grounded = IsGrounded();
        PlayerStateMachine.UpdateState();

        ApplyGravity();
        Move();
        SetRotationToMoveDirection();
    }

    void Move()
    {
        CharacterControllerRef.Move(MoveVector * Time.deltaTime);
    }

    void SetRotationToMoveDirection()
    {
        Vector3 lookDirection = new Vector3(MovementInput.x, 0.0f, MovementInput.y);
        if (lookDirection.magnitude >= 0.1)
        {
            transform.DORotateQuaternion(Quaternion.LookRotation(lookDirection, Vector3.up), 0.4f).SetEase(Ease.OutCirc);
        }
    }

    void ApplyGravity()
    {
        if (Grounded && VelocityGravitational.y < 0.0f) // reset gravitational velocity to 0-ish if grounded
        {
            VelocityGravitational = Vector3.down * 10f;
        }

        MoveVector += VelocityGravitational;
        // print(VelocityGravitational.y);
    }

    public void SetDefaultGravity()
    {
        VelocityGravitational -= new Vector3(0, GravitationalAcceleration * Time.deltaTime, 0);
    }

    bool IsGrounded()
    {
        return (CharacterControllerRef.collisionFlags & CollisionFlags.Below) != 0;
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        float cameraAngle = (CameraRef.transform.rotation.eulerAngles.y) * Mathf.Deg2Rad;
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
            if (!Grounded)
            {
                IsReceivingFlapInput = true;
            }
        }
        else if (context.canceled)
        {
            IsReceivingJumpInput = false;
            IsReceivingFlapInput = false;
            HasStoppedReceivingJumpInput = true;
        }
    }

    public void GetInteractionInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Events.Instance.Interact();
        }
    }


#if UNITY_EDITOR
    [SerializeField] GUIStyle DebugTextStyle;
    private void OnGUI()
    {
        var position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        var textSize = GUI.skin.label.CalcSize(new GUIContent(PlayerStateMachine.CurrentState.ToString()));
        GUI.Label(new Rect(position.x, Screen.height - position.y, textSize.x, textSize.y), PlayerStateMachine.CurrentState.ToString(), DebugTextStyle);
        GUI.Label(new Rect(position.x, Screen.height - position.y + (textSize.y * 2), textSize.x, textSize.y), VelocityGravitational.y.ToString(), DebugTextStyle);
    }
#endif
}