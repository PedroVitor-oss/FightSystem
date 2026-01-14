using UnityEngine;
using System.Collections.Generic;

public class PlayerStateControler : MonoBehaviour
{
    public PlayerState state = PlayerState.Idle;

    public bool inGround = false;


    void Update()
    {
        if(inGround && CheckChangeState(state, new List<PlayerState> { PlayerState.Fall }))//futuramente adicionar climbi, webSwing, etc 
        {
            ChangeState(PlayerState.Idle);
        }
    }


    public bool CheckInGround()
    {
        return inGround;
    }
    public bool ChangeState(PlayerState newState)
    {
        if (newState == state)
            return true;

        switch (state)
        {
            case PlayerState.Idle:
                if (CheckChangeState(newState, new List<PlayerState>
                {
                    PlayerState.Move,
                    PlayerState.Jump,
                    PlayerState.Fight
                }))
                {
                    state = newState;
                    return true;
                }
                break;
            case PlayerState.Move:
                if (CheckChangeState(newState, new List<PlayerState>
                {
                    PlayerState.Idle,
                    PlayerState.Jump,
                    PlayerState.Fight
                }))
                {
                    state = newState;
                    return true;
                }
                break;
            case PlayerState.Jump:
                if (CheckChangeState(newState, new List<PlayerState>
                {
                    PlayerState.Fall
                }))
                {
                    state = newState;
                    return true;
                }
                break;
            case PlayerState.Fall:
                if (CheckChangeState(newState, new List<PlayerState>
                {
                    PlayerState.Idle,
                    PlayerState.Move,
                }) && inGround)
                {
                    state = newState;
                    return true;
                }
                break;
            case PlayerState.Fight:
                if (CheckChangeState(newState, new List<PlayerState>
                {
                    PlayerState.Idle,
                    PlayerState.Move
                }))
                {
                    state = newState;
                    return true;
                }   
                break;
        }

        return false;
    }

    public void ColisionGround()
    {
        inGround = true;
        ChangeState(PlayerState.Idle);
    }
    public void ExitGround()
    {
        inGround = false;
    }
    
    private bool CheckChangeState(PlayerState state, List<PlayerState> list)
    {
        return list.Contains(state);
    }

    public bool CheckState(PlayerState stateCheck)
    {
        return state == stateCheck;
    }
}
