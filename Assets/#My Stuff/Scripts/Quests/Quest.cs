using UnityEngine;

[CreateAssetMenu(menuName = "Joosh/Quest")]
public class Quest : ScriptableObject
{
    bool Completed = false;
    [SerializeField] ObjectiveTrigger[] Objectives;
}