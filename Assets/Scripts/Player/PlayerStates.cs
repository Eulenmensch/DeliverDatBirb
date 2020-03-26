using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : Singleton<PlayerStates>
{
    protected PlayerStates() { }

    public enum MotionStates
    {
        Default,
        Jumping,
        Flapping,
        Gliding,
        Falling
    }

    public bool IsTalking;
    public bool IsCarrying;

    public MotionStates MotionState;

    private void Awake()
    {
        MotionState = MotionStates.Default;
        IsTalking = false;
        IsCarrying = false;
    }
}
