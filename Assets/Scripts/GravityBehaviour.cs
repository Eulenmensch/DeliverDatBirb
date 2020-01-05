using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBehaviour : MonoBehaviour
{
    public float GravitationalAcceleration = 9.81f;
    public GameObject GroundCheckSphere;
    public float GroundCheckRadius;
    public LayerMask GroundLayerMask;

    private Vector3 VelocityGravitational;

    void FixedUpdate()
    {
        ApplyGravity();
    }

    void ApplyGravity()
    {
        if (IsGrounded() && VelocityGravitational.y <= 0)
        {
            VelocityGravitational.y = 0;
        }
        else
        {
            VelocityGravitational.y -= GravitationalAcceleration * Mathf.Pow(Time.deltaTime, 2);
        }

        Vector3 newPosition = transform.position;
        newPosition.y += VelocityGravitational.y;
        transform.position = newPosition;
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(GroundCheckSphere.transform.position, GroundCheckRadius, GroundLayerMask);
    }
}
