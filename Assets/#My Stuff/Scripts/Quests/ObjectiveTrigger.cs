using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    [SerializeField] Objective objective;
    public Objective Objective
    {
        get => objective;
        set => objective = value;
    }

    private bool Active;

    private void OnEnable()
    {
        Events.Instance.OnInteract += SetActive;
    }
    private void OnDisable()
    {
        Events.Instance.OnInteract -= SetActive;
    }

    protected virtual void Interact()
    {

    }

    private void SetActive()
    {
        if (QuestManager.Instance.ActiveObjective == Objective)
        {
            Active = true;
        }
        else
        {
            Active = false;
        }
    }
}