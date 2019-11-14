using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float Speed;

    private CharacterController CharacterController;
    private InputMaster Controls;
    private Vector2 MoveDir;
    private Vector3 CharacterDir;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();

        Controls = new InputMaster();

        Controls.Player.Move.started += context => MoveDir = context.ReadValue<Vector2>();
        Controls.Player.Move.started += context => Move();
        Controls.Player.Move.canceled += context => MoveDir = Vector2.zero;
    }

    private void Update()
    {
        Move();
    }
    public void Move()
    {
        Debug.Log(CharacterDir);
        CharacterDir = transform.right * MoveDir.x + transform.forward * MoveDir.y;
        CharacterDir *= Time.deltaTime;
        CharacterController.Move(CharacterDir * Speed);
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
