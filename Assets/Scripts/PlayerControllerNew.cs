using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerNew : MonoBehaviour
{
    public float Speed;

    float HorizontalInput;
    float VerticalInput;

    void FixedUpdate()
    {
        HandleMoveInput();
    }

    void HandleMoveInput()
    {
        Vector3 newPosition = transform.position;
        newPosition.x += HorizontalInput * Time.deltaTime * Speed;
        newPosition.z += VerticalInput * Time.deltaTime * Speed;
        transform.position = newPosition;
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
}