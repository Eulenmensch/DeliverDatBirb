using UnityEngine;
using System.Linq;

public class BeginDialogueTrigger : ObjectiveTrigger
{
    [SerializeField] Player player;
    [SerializeField] GameObject ButtonPrompt;
    [SerializeField] Transform UITarget;

    private BabyBird Baby;
    private bool PlayerInRange;

    private void OnEnable()
    {
        Events.Instance.OnInteract += Interact;
    }

    private void OnDisable()
    {
        Events.Instance.OnInteract -= Interact;
    }

    private void Start()
    {
        Baby = transform.GetComponentInParent<BabyBird>();
        player = FindObjectOfType<Player>();
    }

    protected override void Interact()
    {
        base.Interact();
        if (PlayerInRange)
        {
            player.ActiveQuest = Baby.Quest;
            player.BabyLookAt = Baby.transform;
            AddYarnProgram(Baby.Dialogue);
            player.DialogueCanvas.transform.parent = UITarget;
            player.DialogueCanvas.transform.localPosition = Vector3.zero;
            player.DialogueCam.LookAt = Baby.transform;
            DialogueManager.Instance.YarnRunner.StartDialogue(Baby.StartNode);
            player.InDialogue = true;
        }
    }

    private void AddYarnProgram(YarnProgram _dialogue)
    {
        if (!DialogueManager.Instance.YarnRunner.yarnScripts.Contains(_dialogue))
        {
            DialogueManager.Instance.YarnRunner.Add(_dialogue);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerInRange = true;
        }
        ButtonPrompt.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            PlayerInRange = false;
        }
        ButtonPrompt.SetActive(false);
    }
}