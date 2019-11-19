using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public float GravitationalAcceleration = 9.81f;
    public bool IsIsometric;
    public GameObject GroundCheckSphere;
    public float GroundCheckRadius;
    public LayerMask GroundLayerMask;

    private CharacterController CharacterController;
    private InputMaster Controls;
    private Vector2 MoveDir;
    private Vector3 CharacterDir;
    private Vector3 VelocityGravitational;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();

        Controls = new InputMaster();

        Controls.Player.Move.performed += context => MoveDir = context.ReadValue<Vector2>();
        Controls.Player.Move.canceled += context => MoveDir = Vector2.zero;
    }

    private void Update()
    {
        Move();
        ApplyGravity();
        Rotate();
    }
    public void Move()
    {
        CharacterDir = transform.right * MoveDir.x + transform.forward * MoveDir.y;
        if (IsIsometric)
        {
            CharacterDir = Quaternion.AngleAxis(45, Vector3.up) * CharacterDir;
        }
        CharacterDir *= Time.deltaTime;
        CharacterController.Move(CharacterDir * Speed);
    }

    void ApplyGravity()
    {
        print(IsGrounded());
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
