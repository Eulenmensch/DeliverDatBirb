using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerNew : MonoBehaviour
{
    private CharacterController CharacterControllerRef;

    public GameObject Camera;
    public Transform GroundCheckSphere;
    public float GroundCheckRadius;
    public LayerMask GroundLayerMask;

    public float Speed;
    public float AirControlFactor;
    public float GravitationalAcceleration;
    public float JumpHeight;
    public float FlapHeight;
    public float FallModifier;
    public float ShortJumpModifier;

    private float HorizontalInput;
    private float VerticalInput;
    private bool IsReceivingJumpInput;
    private bool HasStoppedReceivingJumpInput;

    private Vector3 MoveDirection;
    private Vector3 VelocityGravitational;

    private bool IsJumping;
    private bool IsFlapping;

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

    void HandleJumpInput()
    {
        if (IsGrounded())
        {
            if (IsReceivingJumpInput && !IsJumping && HasStoppedReceivingJumpInput)
            {
                HasStoppedReceivingJumpInput = false;
                IsJumping = true;
            }
            else if (IsJumping) // else if so that if you get jumpinput you don't set it back to false on the same frame
            {
                IsJumping = false;
            }
        }
    }

    void ApplyJumpInput()
    {
        if (IsJumping)
        {
            MoveDirection.y = Mathf.Sqrt(JumpHeight * -2f * -GravitationalAcceleration);
        }
    }

    void HandleFlapInput()
    {
        if (IsReceivingJumpInput && !IsGrounded() && HasStoppedReceivingJumpInput)
        {
            IsFlapping = true;
            VelocityGravitational.y = 0.0f;
        }
        else if (IsGrounded())
        {
            IsFlapping = false;
        }
    }

    void ApplyFlapInput()
    {
        if (IsFlapping)
        {
            IsJumping = false;
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
        else if (IsJumping && !IsReceivingJumpInput && currentVelocityY >= 0) // make the jump shorter if the jump button was released before reaching the apex
        {
            VelocityGravitational.y -= GravitationalAcceleration * ShortJumpModifier * Time.deltaTime;
        }
        else if (IsJumping && currentVelocityY < 0) // make the descent faster than the ascend
        {
            VelocityGravitational.y -= GravitationalAcceleration * FallModifier * Time.deltaTime;
        }
        else // if nothing else, apply normal gravity
        {
            VelocityGravitational.y -= GravitationalAcceleration * Time.deltaTime;
        }
        MoveDirection.y += VelocityGravitational.y;
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(GroundCheckSphere.position, GroundCheckRadius, GroundLayerMask);
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
}