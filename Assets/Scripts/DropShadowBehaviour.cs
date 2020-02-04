using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadowBehaviour : MonoBehaviour
{
    public float YOffset;
    public LayerMask Mask;
    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position, Vector3.down, out hit, Mathf.Infinity, Mask))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y + YOffset, transform.position.z);
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Debug.DrawRay(transform.position, hit.normal, Color.green);
        }
    }
}
