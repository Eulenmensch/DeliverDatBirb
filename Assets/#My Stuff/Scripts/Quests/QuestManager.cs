using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

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

    [SerializeField] Player Player;

    [SerializeField] private Quest activeQuest;
    public Quest ActiveQuest
    {
        get => activeQuest;
        set => activeQuest = value;
    }
    [SerializeField] private Objective activeObjective;
    public Objective ActiveObjective
    {
        get => activeObjective;
        set => activeObjective = value;
    }

    private void OnEnable()
    {
        Events.Instance.OnAcceptQuest += SetActiveQuest;
        Events.Instance.OnAcceptQuest += SetActiveObjective;
        Events.Instance.OnInteract += CheckQuestCompletion;
        Events.Instance.OnInteract += SetActiveObjective;
    }
    private void OnDisable()
    {
        Events.Instance.OnAcceptQuest -= SetActiveQuest;
        Events.Instance.OnAcceptQuest -= SetActiveObjective;
        Events.Instance.OnInteract -= CheckQuestCompletion;
        Events.Instance.OnInteract -= SetActiveObjective;
    }

    private void Start()
    {
        Events.Instance.AcceptQuest(); //HACK: This is temporary before quests can be accepted for real
    }

    private void SetActiveQuest()
    {
        ActiveQuest = Player.ActiveQuest;
    }

    private void CheckQuestCompletion()
    {
        ActiveQuest.CheckCompleted();
        if (ActiveQuest.Completed)
        {
            Events.Instance.CompleteQuest();
        }
    }

    private void SetActiveObjective()
    {
        foreach (var objective in ActiveQuest.Objectives)
        {
            if (!objective.Completed)
            {
                ActiveObjective = objective;
            }
        }
    }
}