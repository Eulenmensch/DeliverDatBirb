using UnityEngine;

public class PlayerIdleState : IState
{
    Player Owner;

    public PlayerIdleState(Player _owner)
    {
        Owner = _owner;
    }

    public void Enter() { }

    public void Execute()
    {
        ApplyGravity();

        CheckStateChanges();
    }

    public void Exit() { }

    void ApplyGravity() //FIXME: Make this a public function in player "ApplyRegularGravity" or something as this code is needed by several states.
    {
        Owner.VelocityGravitational = new Vector3(0, Owner.VelocityGravitational.y - Owner.GravitationalAcceleration * Time.deltaTime, 0);

        Owner.MoveVector = new Vector3(Owner.MoveVector.x, Owner.MoveVector.y + Owner.VelocityGravitational.y, Owner.MoveVector.z);
    }

    private void CheckStateChanges()
    {
        if (Owner.MovementInput.magnitude >= 0f)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerWalkingState(Owner));
        }
        if (Owner.IsReceivingJumpInput)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerJumpingState(Owner));
        }
        if (Owner.InDialogue)
        {
            Owner.PlayerStateMachine.ChangeState(new PlayerDialogueState(Owner));
        }
    }
}