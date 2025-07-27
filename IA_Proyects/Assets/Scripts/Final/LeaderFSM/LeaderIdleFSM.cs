using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderIdleFSM : IState
{
    LeaderFSM _fsm;
    Leader _myLeader;
    Action Idle;

    public LeaderIdleFSM(LeaderFSM fsm, Leader myLeader, Action idle)
    {
        _fsm = fsm;
        _myLeader = myLeader;
        Idle = idle;
    }

    public void OnEnter()
    {
        _myLeader.State = LeaderState.Idle;
        _myLeader.UpdateColor();

    }

    public void OnUpdate()
    {
        Idle();
    }

    public void OnExit()
    {

    }
}
