using UnityEngine;

public class PlayerWalkingState : IState
{
    Player Owner;

    public PlayerWalkingState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter() { }
    public void Execute()
    {
        SetMoveVector();
        SetGravity();
        SetAnimatorSpeedFloat();
        CheckStateChanges();
    }

    public void Exit() { }

    void SetMoveVector()
    {
        Vector3 inputVector = new Vector3(Owner.MovementInput.x, 0.0f, Owner.MovementInput.y);

        Owner.MoveVector = inputVector;
        Owner.MoveVector = Vector3.ClampMagnitude(Owner.MoveVector, 1);
        Owner.MoveVector *= Owner.Speed;
    }

    void SetGravity()
    {
        Owner.SetDefaultGravity();
    }

    private void SetAnimatorSpeedFloat()
    {
        Vector3 horizontalVelocity = new Vector3(Owner.CharacterControllerRef.velocity.x, 0, Owner.CharacterControllerRef.velocity.z);
        float speed = horizontalVelocity.magnitude / Owner.Speed;
        Owner.PlayerAnimator.SetFloat("Speed", speed);
    }

    private void CheckStateChanges()
    {
        if (Owner.MovementInput.magnitude == 0)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerIdleState(Owner));
        }
        if (Owner.IsReceivingJumpInput && Owner.HasStoppedReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerJumpingState(Owner));
        }
        if (!Owner.Grounded)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFallingState(Owner));
        }
        if (Owner.InDialogue)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerDialogueState(Owner));
        }
    }
}