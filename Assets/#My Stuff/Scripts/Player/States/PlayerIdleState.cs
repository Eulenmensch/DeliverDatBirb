using UnityEngine;

public class PlayerIdleState : IState
{
    Player Owner;

    public PlayerIdleState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter()
    {
        Owner.PlayerAnimator.SetFloat("Speed", 0f);
        Owner.PlayerAnimator.SetBool("Jumping", false);
        Owner.MoveVector = Vector3.zero;
    }

    public void Execute()
    {
        SetGravity();

        CheckStateChanges();
    }

    public void Exit() { }

    void SetGravity()
    {
        Owner.SetDefaultGravity();
    }

    private void CheckStateChanges()
    {
        if (Owner.MovementInput.magnitude > 0f)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerWalkingState(Owner));
        }
        if (Owner.IsReceivingJumpInput && Owner.HasStoppedReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerJumpingState(Owner));
        }
        if (Owner.InDialogue)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerDialogueState(Owner));
        }
    }
}