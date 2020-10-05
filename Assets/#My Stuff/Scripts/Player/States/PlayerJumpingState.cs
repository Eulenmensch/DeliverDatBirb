using UnityEngine;

public class PlayerJumpingState : IState
{
    Player Owner;

    float InitialHeight;

    public PlayerJumpingState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter()
    {
        InitialHeight = Owner.transform.position.y;
        Owner.HasStoppedReceivingJumpInput = false;
        Owner.PlayerAnimator.SetBool("Jumping", true);
        Owner.VelocityGravitational = Vector3.up * 1f; //HACK:I'm pushing down the character when grounded to compensate for a jittery grounded check so when jumping I need some upwards velocity
        ApplyJumpInput();
    }
    public void Execute()
    {
        SetGlideMovement();
        ApplyJumpInput();
        SetGravity();
        CheckStateChanges();
    }


    public void Exit()
    {
    }

    void ApplyJumpInput()
    {
        float moveY = Mathf.Sqrt(Owner.JumpHeight * -2f * -Owner.GravitationalAcceleration);
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
        if (Owner.IsReceivingFlapInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFlappingState(Owner));
        }
        if (Owner.HasStoppedReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerFallingState(Owner));
        }
        if (Owner.CharacterControllerRef.velocity.y < 0)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerGlidingState(Owner));
        }
    }
}