using UnityEngine;

public class PlayerDialogueState : IState
{
    Player Owner;

    public PlayerDialogueState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter()
    {
        Owner.DialogueCam.gameObject.SetActive(true);
        Owner.transform.LookAt(new Vector3(Owner.BabyLookAt.position.x, Owner.transform.position.y, Owner.BabyLookAt.position.z), Vector3.up);
    }

    public void Execute()
    {
        CheckStateChanges();
        Owner.transform.LookAt(new Vector3(Owner.BabyLookAt.position.x, Owner.transform.position.y, Owner.BabyLookAt.position.z), Vector3.up);
    }

    public void Exit()
    {
        Owner.DialogueCam.gameObject.SetActive(false);
    }

    public void CheckStateChanges()
    {
        if (!Owner.InDialogue)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerIdleState(Owner));
        }
    }
}