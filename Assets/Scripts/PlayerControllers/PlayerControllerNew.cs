using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerNew : MonoBehaviour
{
    private CharacterController CharacterControllerRef;

    public GameObject Camera;
    public GameObject GroundCheckSphere;
    public float GroundCheckRadius;
    public LayerMask GroundLayerMask;

    public float Speed;
    public float AirControlFactor;
    public float JumpHeight;
    public float GravitationalAcceleration;
    public float FallModifier;
    public float ShortJumpModifier;

    private float HorizontalInput;
    private float VerticalInput;
    private bool IsReceivingJumpInput;

    private Vector3 MoveDirection;
    private Vector3 VelocityGravitational;

    private bool IsJumping;

    private void Start()
    {
        CharacterControllerRef = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        ApplyMoveInput();
        HandleJumpInput();
        ApplyJumpInput();
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
            MoveDirection = MoveDirection.normalized;
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
        if (IsReceivingJumpInput && !IsJumping)
        {
            IsJumping = true;
        }
        else if (IsGrounded() && IsJumping)
        {
            IsJumping = false;
        }
    }

    void ApplyJumpInput()
    {
        if (IsJumping) // This should probably be an IsJumping Bool that is true as long as the character is in the air
        {
            MoveDirection.y = Mathf.Sqrt(JumpHeight * -2f * -GravitationalAcceleration);
        }
    }

    void ApplyGravity()
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
        return Physics.CheckSphere(GroundCheckSphere.transform.position, GroundCheckRadius, GroundLayerMask);
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
        }
    }
}