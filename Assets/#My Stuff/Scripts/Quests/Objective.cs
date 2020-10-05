using UnityEngine;

public class Objective : MonoBehaviour
{
    [SerializeField] ObjectiveTrigger trigger;
    public ObjectiveTrigger Trigger
    {
        get { return trigger; }
        set { trigger = value; }
    }
}