using UnityEngine;

public class PlayerGlidingState : IState
{
    Player Owner;

    public PlayerGlidingState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter()
    {
        Owner.VelocityGravitational = Vector3.zero;
    }
    public void Execute()
    {
        SetGlideMovement();
        SetGravity();

        CheckStateChanges();
    }

    public void Exit() { }

    private void SetGlideMovement()
    {
        Vector3 inputVector = new Vector3(Owner.MovementInput.x, 0.0f, Owner.MovementInput.y);

        Owner.MoveVector = new Vector3(Owner.MoveVector.x, 0f, Owner.MoveVector.z);     // reset the move vector to ground plane
        Owner.MoveVector += inputVector * Owner.AirControlFactor;                       // gives aircontrol dependent on AirControlFactor
        Owner.MoveVector = Vector3.ClampMagnitude(Owner.MoveVector, Owner.Speed);       // limits the movedirection's speed to the defined movespeed
    }

    private void SetGravity()
    {
        Owner.VelocityGravitational -= new Vector3(0, Owner.GravitationalAcceleration * Owner.GlideModifier * Time.deltaTime, 0);
    }

    public void CheckStateChanges()
    {
        if (Owner.HasStoppedReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFallingState(Owner));
        }
        if (Owner.Grounded)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerIdleState(Owner));
        }
    }
}