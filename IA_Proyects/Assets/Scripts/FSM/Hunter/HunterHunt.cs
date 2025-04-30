using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HunterHunt : IState
{
    FSM _fsm;
    Hunter _myHunter;
    SpriteRenderer _renderer;

    Action<Vector3> AddForce;
    Func<Agent, Vector3> Persuit;


    public HunterHunt(FSM fsm, Hunter myHunter, SpriteRenderer render, Action<Vector3> newAddForce, Func<Agent, Vector3> newPersuit)
    {
        _fsm = fsm;
        _myHunter = myHunter;
        _renderer = render;
        AddForce = newAddForce;
        Persuit = newPersuit;
    }

    public void OnEnter()
    {
        _renderer.color = Color.red;
        //_myHunter.hunterState = AgentState.Rest;

    }

    public void OnExit()
    {
        _renderer.color = Color.black;
    }

    public void OnUpdate()
    {
        //a
        AddForce(Persuit(_myHunter.closeBoid));

        if (Vector3.Distance(_myHunter.transform.position, _myHunter.closeBoid.transform.position) < 0.2f)
        {
            _myHunter.closeBoid.GetEaten();
            _fsm.ChangeState(AgentState.Rest);
        }
    }
}
