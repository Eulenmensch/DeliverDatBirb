using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadowBehaviour : MonoBehaviour
{
    public float YOffset;
    public Transform Player;
    public LayerMask Mask;
    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Player.position, Vector3.down, out hit, Mathf.Infinity, Mask))
        {
            //transform.position = new Vector3(transform.position.x, hit.point.y + YOffset, transform.position.z);
            //transform.position = hit.point;
            transform.position = new Vector3(hit.point.x, hit.point.y + YOffset, hit.point.z);
            transform.rotation = Quaternion.LookRotation(-hit.normal);
            Debug.DrawRay(transform.position, hit.normal, Color.green);
        }
    }
}
