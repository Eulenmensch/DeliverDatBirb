using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float JumpHeight;
    public float GravitationalAcceleration = 9.81f;
    public bool IsIsometric; //FIXME: This needs to be refactored out once it is certain which camera projection we are using
    public GameObject GroundCheckSphere;
    public float GroundCheckRadius;
    public LayerMask GroundLayerMask;

    private CharacterController CharacterController;
    private InputMaster Controls;
    private Vector2 MoveDir;
    private Vector3 CharacterDir;
    private Vector3 VelocityGravitational;
    private bool IsReceivingJumpInput;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();

        Controls = new InputMaster();

        Controls.Player.Move.performed += context => MoveDir = context.ReadValue<Vector2>();
        Controls.Player.Move.canceled += context => MoveDir = Vector2.zero;
        Controls.Player.Jump.performed += context => IsReceivingJumpInput = true;
        Controls.Player.Jump.canceled += context => IsReceivingJumpInput = false;
    }

    private void FixedUpdate()
    {
        Move();
        // ApplyGravity();
        Jump();
        // Rotate();
        CharacterController.Move(CharacterDir);
    }
    private void Move()
    {
        CharacterDir = transform.right * MoveDir.x + transform.forward * MoveDir.y;
        if (IsIsometric)
        {
            CharacterDir = Quaternion.AngleAxis(45, Vector3.up) * CharacterDir;
        }
        CharacterDir *= Time.deltaTime;
        CharacterDir *= Speed;
    }

    void Jump()
    {
        if (IsReceivingJumpInput && IsGrounded())
        {
            CharacterDir.y = Mathf.Sqrt(JumpHeight * -2f * -GravitationalAcceleration);
            print(Mathf.Sqrt(JumpHeight * 2f * GravitationalAcceleration));
        }
    }

    void ApplyGravity()
    {
        if (IsGrounded() && VelocityGravitational.y < 0)
        {
            VelocityGravitational.y = 0;
        }
        VelocityGravitational.y -= GravitationalAcceleration * Mathf.Pow(Time.deltaTime, 2);
        CharacterController.Move(VelocityGravitational);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(GroundCheckSphere.transform.position, GroundCheckRadius, GroundLayerMask);
    }

    void Rotate()
    {
        if (!IsIsometric)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void OnEnable()
    {
        Controls.Player.Enable();
    }
    private void OnDisable()
    {
        Controls.Player.Disable();
    }
}
