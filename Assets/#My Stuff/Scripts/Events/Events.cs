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

    public event Action OnTriggerObjective;
    public void TriggerObjective() { OnTriggerObjective?.Invoke(); }

    public event Action OnAcceptQuest;
    public void AcceptQuest() { OnAcceptQuest?.Invoke(); }

    public event Action OnCompleteQuest;
    public void CompleteQuest() { OnCompleteQuest?.Invoke(); }
}