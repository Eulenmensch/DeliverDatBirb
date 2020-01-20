using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerNew : MonoBehaviour
{
    private CharacterController CharacterControllerRef;

    public float Speed;
    public float JumpHeight;
    public float GravitationalAcceleration;

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
        ApplyGravity();
        Debug.DrawRay(transform.position, MoveDirection, Color.magenta); //TODO Remove this after debugging
        CharacterControllerRef.Move(MoveDirection * Time.deltaTime);
    }

    void ApplyMoveInput()
    {
        MoveDirection = new Vector3(HorizontalInput, 0.0f, VerticalInput);
        MoveDirection = MoveDirection.normalized;
        MoveDirection *= Speed;
    }

    void HandleJumpInput()
    {
        if (IsReceivingJumpInput) // This should probably be an IsJumping Bool that is true as long as the character is in the air
        {
            MoveDirection.y = Mathf.Sqrt(JumpHeight * -2f * -GravitationalAcceleration);
        }
    }

    void ApplyJumpInput()
    {
        if (IsReceivingJumpInput && !IsJumping)
        {
            //do jumpy stuff
            IsJumping = true;
        }

    }

    void ApplyGravity()
    {
        if (CharacterControllerRef.isGrounded && VelocityGravitational.y <= 0.0f)
        {
            VelocityGravitational.y = 0.0f;
        }
        else
        {
            VelocityGravitational.y -= GravitationalAcceleration * Time.deltaTime;
        }
        MoveDirection.y += VelocityGravitational.y;
    }

    public void GetMoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        Vector2 rotatedInputVector = new Vector2(
            inputVector.x * Mathf.Cos(-0.7854f) - inputVector.y * Mathf.Sin(-0.7854f),
            inputVector.x * Mathf.Sin(-0.7854f) + inputVector.y * Mathf.Cos(-0.7854f)
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