using UnityEngine;

public class TestObjective : Objective
{
    [SerializeField] Canvas ButtonPromptCanvas;
    bool Active;

    private void OnEnable()
    {
        Events.Instance.OnInteract += Interaction;
    }

    private void OnDisable()
    {
        Events.Instance.OnInteract -= Interaction;
    }

    private void Interaction()
    {
        if (Active)
        {
            Trigger.Completed = true;
            print(Trigger.ObjectiveCompletionText[0]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ButtonPromptCanvas.gameObject.SetActive(true);
        Active = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Active = false;
        ButtonPromptCanvas.gameObject.SetActive(false);
    }
}