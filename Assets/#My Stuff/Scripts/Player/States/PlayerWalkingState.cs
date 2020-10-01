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
        ApplyMoveInput();
        ApplyGravity();
        CheckStateChanges();
    }

    public void Exit() { }

    void ApplyMoveInput()
    {
        Vector3 inputVector = new Vector3(Owner.MovementInput.x, 0.0f, Owner.MovementInput.y);

        Owner.MoveVector = inputVector;
        Owner.MoveVector = Vector3.ClampMagnitude(Owner.MoveVector, 1);
        Owner.MoveVector *= Owner.Speed;
    }

    void ApplyGravity()
    {
        Owner.VelocityGravitational = new Vector3(0, Owner.VelocityGravitational.y - Owner.GravitationalAcceleration * Time.deltaTime, 0);

        Owner.MoveVector = new Vector3(Owner.MoveVector.x, Owner.MoveVector.y + Owner.VelocityGravitational.y, Owner.MoveVector.z);
    }

    private void CheckStateChanges()
    {
        if (Owner.MovementInput.magnitude == 0)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerIdleState(Owner));
        }
        if (Owner.IsReceivingJumpInput)
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