using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    [SerializeField] Objective objective;
    public Objective Objective
    {
        get => objective;
        set => objective = value;
    }

    public bool Active { get; private set; }

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