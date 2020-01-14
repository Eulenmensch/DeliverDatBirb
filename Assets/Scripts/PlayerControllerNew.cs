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
    private bool IsJumping;

    private void Start()
    {
        CharacterControllerRef = GetComponent<CharacterController>();
    }

    void FixedUpdate()
    {
        HandleMoveInput();
        HandleJumpInput();
        ApplyGravity();
        CharacterControllerRef.Move(MoveDirection * Time.deltaTime);
    }

    void HandleMoveInput()
    {
        MoveDirection = new Vector3(HorizontalInput, 0.0f, VerticalInput);
        MoveDirection *= Speed;
    }

    void HandleJumpInput()
    {
        if (IsJumping && CharacterControllerRef.isGrounded)
        {
            IsJumping = false;
            IsReceivingJumpInput = false;
        }
        if (IsReceivingJumpInput)
        {
            MoveDirection.y = Mathf.Sqrt(JumpHeight * -2f * -GravitationalAcceleration);
            // MoveDirection.y = JumpHeight;
            IsJumping = true;
        }
    }

    void ApplyGravity()
    {
        // if (CharacterControllerRef.isGrounded && MoveDirection.y < 0)
        // {
        //     MoveDirection.y = 0;
        // }
        MoveDirection.y -= GravitationalAcceleration * Time.deltaTime;
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
        if (CharacterControllerRef.isGrounded)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                IsReceivingJumpInput = true;
            }
        }
        else
        {
            IsReceivingJumpInput = false;
        }
    }
}