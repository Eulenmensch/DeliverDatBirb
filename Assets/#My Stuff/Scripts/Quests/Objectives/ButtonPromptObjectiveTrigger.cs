using UnityEngine;

public class ButtonPromptObjectiveTrigger : ObjectiveTrigger
{
    [SerializeField] Canvas ButtonPromptCanvas = null;
    bool CanvasActive;

    private void OnEnable()
    {
        Events.Instance.OnInteract += Interact;
    }

    private void OnDisable()
    {
        Events.Instance.OnInteract -= Interact;
    }

    protected override void Interact()
    {
        base.Interact();
        if (CanvasActive)
        {
            Objective.SetCompleted();
            Debug.Log(Objective.ObjectiveCompletionTexts[0]);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ButtonPromptCanvas.gameObject.SetActive(true);
        CanvasActive = true;
    }

    private void OnTriggerExit(Collider other)
    {
        CanvasActive = false;
        ButtonPromptCanvas.gameObject.SetActive(false);
    }
}