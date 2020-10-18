using UnityEngine;

[CreateAssetMenu(menuName = "Joosh/Objective")]
public class Objective : ScriptableObject
{
    public bool Completed { get; private set; } = false;
    public string[] BabyObjectiveTexts
    {
        get { return babyObjectiveTexts; }
        private set { babyObjectiveTexts = value; }
    }
    [SerializeField, TextArea] string[] babyObjectiveTexts;

    public string[] ObjectiveCompletionTexts
    {
        get { return objectiveCompletionTexts; }
        private set { objectiveCompletionTexts = value; }
    }
    [SerializeField, TextArea] string[] objectiveCompletionTexts;

    public void SetCompleted()
    {
        Completed = true;
    }
}