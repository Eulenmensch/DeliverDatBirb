using UnityEngine;

[CreateAssetMenu(menuName = "Joosh/ObjectiveTrigger")]
public class ObjectiveTrigger : ScriptableObject
{
    public bool Completed { get; set; } = false;
    public string[] BabyObjectiveTexts
    {
        get { return babyObjectiveTexts; }
        private set { babyObjectiveTexts = value; }
    }
    [SerializeField, TextArea] string[] babyObjectiveTexts;

    public string[] ObjectiveCompletionText
    {
        get { return objectiveCompletionText; }
        private set { objectiveCompletionText = value; }
    }
    [SerializeField, TextArea] string[] objectiveCompletionText;

    public void SetCompleted()
    {
        Completed = true;
    }
}