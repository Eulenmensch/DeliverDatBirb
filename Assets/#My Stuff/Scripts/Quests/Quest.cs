using UnityEngine;

[CreateAssetMenu(menuName = "Joosh/Quest"), System.Serializable]
public class Quest : ScriptableObject
{
    [SerializeField] Objective[] objectives = null;
    public Objective[] Objectives
    {
        get => objectives;
        set => objectives = value;
    }

    public bool Completed { get; private set; }

    public void CheckCompleted()
    {
        foreach (var objective in Objectives)
        {
            if (objective.Completed) { return; }
        }
        Completed = true;
    }
}