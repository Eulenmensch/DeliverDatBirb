using UnityEngine;

public class RotateToFaceCamera : MonoBehaviour
{
    private Camera MainCamera;

    private void Start()
    {
        MainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + MainCamera.transform.rotation * Vector3.forward, MainCamera.transform.rotation * Vector3.up);
    }
}