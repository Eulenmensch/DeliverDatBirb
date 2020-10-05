using UnityEngine;

public class PlayerFlappingState : IState
{
    Player Owner;

    public PlayerFlappingState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter()
    {
        Owner.PlayerAnimator.SetTrigger("Flap");
        Owner.HasStoppedReceivingJumpInput = false;

        Owner.VelocityGravitational = Vector3.zero;
        ApplyFlapInput();
    }

    public void Execute()
    {
        SetGlideMovement();
        ApplyFlapInput();
        SetGravity();

        CheckStateChanges();
    }

    public void Exit() { }

    private void ApplyFlapInput()
    {
        float moveY = Mathf.Sqrt(Owner.FlapHeight * -2f * -Owner.GravitationalAcceleration);
        Owner.MoveVector += new Vector3(0, moveY, 0);
    }

    private void SetGlideMovement()
    {
        Vector3 inputVector = new Vector3(Owner.MovementInput.x, 0.0f, Owner.MovementInput.y);

        Owner.MoveVector = new Vector3(Owner.MoveVector.x, 0f, Owner.MoveVector.z);     // reset the move vector to ground plane
        Owner.MoveVector += inputVector * Owner.AirControlFactor;                       // gives aircontrol dependent on AirControlFactor
        Owner.MoveVector = Vector3.ClampMagnitude(Owner.MoveVector, Owner.Speed);       // limits the movedirection's speed to the defined movespeed
    }

    private void SetGravity()
    {
        Owner.SetDefaultGravity();
    }

    private void CheckStateChanges()
    {
        if (Owner.CharacterControllerRef.velocity.y < -0.5f)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerGlidingState(Owner));
        }
        if (Owner.HasStoppedReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFallingState(Owner));
        }
    }
}