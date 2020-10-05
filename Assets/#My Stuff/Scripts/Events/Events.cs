using UnityEngine;
using System;

public class Events : MonoBehaviour
{
    public static Events Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public event Action OnInteract;
    public void Interact() { OnInteract?.Invoke(); }
}