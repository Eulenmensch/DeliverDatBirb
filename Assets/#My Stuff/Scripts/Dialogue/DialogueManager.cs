using UnityEngine;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

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

    [SerializeField] DialogueRunner yarnRunner;
    public DialogueRunner YarnRunner { get => yarnRunner; private set => yarnRunner = value; }
    [SerializeField] DialogueUI yarnUI;
    public DialogueUI YarnUI { get => yarnUI; private set => yarnUI = value; }
}