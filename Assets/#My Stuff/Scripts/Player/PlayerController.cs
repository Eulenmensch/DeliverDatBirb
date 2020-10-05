using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private CharacterController CharacterControllerRef;

    public GameObject Camera;
    [SerializeField] Animator PlayerAnimator;
    // public Transform GroundCheckSphere;
    // public float GroundCheckRadius;
    // public LayerMask GroundLayerMask;

    public float Speed;
    public float AirControlFactor;
    public float GravitationalAcceleration;
    public float JumpHeight;
    public float FlapHeight;
    public float FallModifier;
    public float ShortJumpModifier;
    public float GlideModifier;

    private float HorizontalInput;
    private float VerticalInput;
    private bool IsReceivingJumpInput;
    private bool HasStoppedReceivingJumpInput;
    private bool IsReceivingFlapInput;

    private Vector3 MoveDirection;
    private Vector3 VelocityGravitational;


    private void Start()
    {
        CharacterControllerRef = GetComponent<CharacterController>();
        HasStoppedReceivingJumpInput = true; // set the start value to true to start to not eat the first jump input
        IsReceivingJumpInput = false;
    }

    void FixedUpdate()
    {
        ApplyMoveInput();
        HandleJumpInput();
        ApplyJumpInput();
        HandleFlapInput();
        ApplyFlapInput();
        ApplyGravity();
        SetRotationToMoveDirection();
        SetAnimatorSpeedFloat();
        Debug.DrawRay(transform.position, MoveDirection, Color.magenta); //TODO Remove this after debugging
        CharacterControllerRef.Move(MoveDirection * Time.deltaTime);
    }

    void ApplyMoveInput()
    {
        Vector3 inputVector = new Vector3(HorizontalInput, 0.0f, VerticalInput);
        if (IsGrounded()) // Grounded Movement
        {
            MoveDirection = inputVector;
            MoveDirection = Vector3.ClampMagnitude(MoveDirection, 1);
            MoveDirection *= Speed;
        }
        else if (!IsGrounded()) // Aerial Movement
        {
            MoveDirection = new Vector3(MoveDirection.x, 0f, MoveDirection.z); // reset the move vector to ground plane
            MoveDirection += inputVector * AirControlFactor; // gives aircontrol dependent on AirControlFactor
            MoveDirection = Vector3.ClampMagnitude(MoveDirection, Speed); // limits the movedirection's speed to the defined movespeed
        }
    }

    private void SetAnimatorSpeedFloat()
    {
        Vector3 horizontalVelocity = new Vector3(CharacterControllerRef.velocity.x, 0, CharacterControllerRef.velocity.z);
        float speed = horizontalVelocity.magnitude / Speed;
        PlayerAnimator.SetFloat("Speed", speed);
    }

    void HandleJumpInput()
    {
        if (IsGrounded())
        {
            if (IsReceivingJumpInput && PlayerStates.Instance.MotionState == PlayerStates.MotionStates.Default && HasStoppedReceivingJumpInput)
            {
                HasStoppedReceivingJumpInput = false;
                PlayerStates.Instance.MotionState = PlayerStates.MotionStates.Jumping;
                PlayerAnimator.SetBool("Jumping", true);
            }
            else if (PlayerStates.Instance.MotionState == PlayerStates.MotionStates.Jumping)
            {
                PlayerStates.Instance.MotionState = PlayerStates.MotionStates.Default;
                PlayerAnimator.SetBool("Jumping", false);
            }
        }
    }

    void ApplyJumpInput()
    {
        if (PlayerStates.Instance.MotionState == PlayerStates.MotionStates.Jumping)
        {
            MoveDirection.y = Mathf.Sqrt(JumpHeight * -2f * -GravitationalAcceleration);
        }
    }

    void HandleFlapInput()
    {
        if (IsReceivingFlapInput && !IsGrounded())
        {
            HasStoppedReceivingJumpInput = false; //to prevent an unintended jump when keeping the jump button held after a flap
            PlayerStates.Instance.MotionState = PlayerStates.MotionStates.Flapping;
            VelocityGravitational.y = 0.0f;
            IsReceivingFlapInput = false;
        }
        else if (IsGrounded() && PlayerStates.Instance.MotionState == PlayerStates.MotionStates.Flapping)
        {
            PlayerStates.Instance.MotionState = PlayerStates.MotionStates.Default;
        }
    }

    void ApplyFlapInput()
    {
        if (PlayerStates.Instance.MotionState == PlayerStates.MotionStates.Flapping)
        {
            MoveDirection.y = Mathf.Sqrt(FlapHeight * -2f * -GravitationalAcceleration);
        }
    }

    void ApplyGravity() //TODO Flapping also needs modifiable falling values. Either make a function that checks is airborne (jumping or flapping) or make more else ifs for flapping specifically.
    {
        float currentVelocityY = MoveDirection.y + VelocityGravitational.y;
        if (CharacterControllerRef.isGrounded && VelocityGravitational.y <= 0.0f) // reset gravitational velocity to 0 if grounded
        {
            VelocityGravitational.y = 0.0f;
        }
        else if (PlayerStates.Instance.MotionState == PlayerStates.MotionStates.Jumping && !IsReceivingJumpInput && currentVelocityY >= 0) // make the jump shorter if the jump button was released before reaching the apex
        {
            VelocityGravitational.y -= GravitationalAcceleration * ShortJumpModifier * Time.deltaTime;
        }
        else if (!IsGrounded() && IsReceivingJumpInput && currentVelocityY < 0) // make the descent slower if the jump button is being held
        {
            VelocityGravitational.y -= GravitationalAcceleration * GlideModifier * Time.deltaTime;
        }
        else if (!IsGrounded() && currentVelocityY < 0) // make the descent faster than the ascend
        {
            print("boop");
            VelocityGravitational.y -= GravitationalAcceleration * FallModifier * Time.deltaTime;
        }
        else // if nothing else, apply normal gravity
        {
            print("regular beep");
            VelocityGravitational.y -= GravitationalAcceleration * Time.deltaTime;
        }
        MoveDirection.y += VelocityGravitational.y;
    }

    void SetRotationToMoveDirection()
    {
        Vector3 lookDirection = new Vector3(HorizontalInput, 0.0f, VerticalInput);
        if (lookDirection.magnitude >= 0.1)
        {
            transform.DORotateQuaternion(Quaternion.LookRotation(lookDirection, Vector3.up), 0.4f).SetEase(Ease.OutCirc);
        }
    }

    bool IsGrounded()
    {
        // return Physics.CheckSphere(GroundCheckSphere.position, GroundCheckRadius, GroundLayerMask);
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
        HorizontalInput = rotatedInputVector.x;
        VerticalInput = rotatedInputVector.y;
    }

    public void GetJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && !IsReceivingJumpInput)
        {
            IsReceivingJumpInput = true;
        }
        else if (context.performed && IsReceivingJumpInput)
        {
            IsReceivingJumpInput = false;
            HasStoppedReceivingJumpInput = true;
        }
    }

    public void GetFlapInput(InputAction.CallbackContext context)
    {
        if (context.started && !IsGrounded())
        {
            IsReceivingFlapInput = true;
        }
    }
}