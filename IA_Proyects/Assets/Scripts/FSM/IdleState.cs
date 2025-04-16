using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    FSM _fsm;
    SpriteRenderer _spriteR;

    public IdleState(FSM newSfm, SpriteRenderer newR)
    {
        _fsm = newSfm;
        _spriteR = newR;
    }

    public void OnEnter()
    {
        _spriteR.material.color = Color.white;
    }

    public void OnExit()
    {
        _spriteR.material.color = Color.red;

    }

    public void OnUpdate()
    {
        if(Input.GetKeyDown(KeyCode.W))
            _fsm.ChangeState(AgentState.Movement);
    }
}
