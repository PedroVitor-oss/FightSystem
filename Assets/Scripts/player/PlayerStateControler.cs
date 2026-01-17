using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerStateControler : MonoBehaviour
{
    public PlayerState state = PlayerState.Idle;

    public bool inGround = false;


    void Update()
    {
        if (inGround && CheckChangeState(state, new List<PlayerState> { PlayerState.Fall }))//futuramente adicionar climbi, webSwing, etc 
        {
            Debug.Log("esta sendo chamando aqui?");
            ChangeState(PlayerState.Idle);
        }
        else if (!inGround && CheckChangeState(state, new List<PlayerState> { PlayerState.Idle, PlayerState.Move, PlayerState.Fight }))
        {
            ChangeState(PlayerState.Fall);
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

        // if (state == PlayerState.Fall && newState == PlayerState.Move)
        // {
        //     EditorApplication.isPaused = true;
        //     Debug.Log("fall to move state, isground: " + inGround);
        // }
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
                    if (newState == PlayerState.Jump)
                    {
                        inGround = false;
                    }
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
                    if (newState == PlayerState.Jump)
                    {
                        inGround = false;
                    }
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
                if (inGround)
                {
                    Debug.Log("trocar de fall para idle ou move: " + newState);
                    // EditorApplication.isPaused = true;
                    if (CheckChangeState(newState, new List<PlayerState>
                    {
                        PlayerState.Idle,
                        PlayerState.Move,

                    }))
                    {
                        state = newState;
                        return true;
                    }
                }
                else
                {
                    Debug.Log("trocar de fall para climb ou runwall: " + newState);
                    // EditorApplication.isPaused = true;
                    if (CheckChangeState(newState, new List<PlayerState>
                    {
                        PlayerState.Climb,
                        PlayerState.RunWall,
                    }))
                    {
                        state = newState;
                        return true;
                    }
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
            case PlayerState.Climb:
                if (CheckChangeState(newState, new List<PlayerState>
                {
                    PlayerState.Fall
                }))
                {
                    state = newState;
                    return true;
                }
                break;
            case PlayerState.RunWall:
            Debug.Log("[PLayerStateControler] run wall to "+newState);
                if (CheckChangeState(newState, new List<PlayerState>
                {
                    PlayerState.Fall
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
