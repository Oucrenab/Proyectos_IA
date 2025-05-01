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
    Func<Vector3, bool> FovCheck;


    public HunterHunt(FSM fsm, Hunter myHunter, SpriteRenderer render, Action<Vector3> newAddForce, Func<Agent, Vector3> newPersuit, Func<Vector3, bool> fovCheck)
    {
        _fsm = fsm;
        _myHunter = myHunter;
        _renderer = render;

        AddForce = newAddForce;
        Persuit = newPersuit;
        FovCheck = fovCheck;
    }

    public void OnEnter()
    {
        _renderer.color = Color.red;
        _myHunter.ViewAngle = 360;
        //_myHunter.hunterState = AgentState.Rest;

    }

    public void OnExit()
    {
        _renderer.color = Color.black;
        _myHunter.ResetViewAngle();
    }

    public void OnUpdate()
    {
        //a

        if (_myHunter.Energy > 0)
        {
            _myHunter.ConsumeEnergy(Time.deltaTime);
        }
        else
        {
            _fsm.ChangeState(AgentState.Rest);
            return;
        }

        if (!FovCheck(_myHunter.closeBoid.transform.position))
        {
            _fsm.ChangeState(AgentState.Patrol);
            return;
        }


        AddForce(Persuit(_myHunter.closeBoid));

        if (Vector3.Distance(_myHunter.transform.position, _myHunter.closeBoid.transform.position) < 0.2f)
        {
            _myHunter.closeBoid.GetEaten();
            _fsm.ChangeState(AgentState.Rest);
        }
    }
}
