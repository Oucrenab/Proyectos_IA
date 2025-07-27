using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderRunFSM : IState
{
    LeaderFSM _fsm;
    Leader _myLeader;
    Action RunAway;

    public LeaderRunFSM(LeaderFSM fsm, Leader myLeader, Action runAway)
    {
        _fsm = fsm;
        _myLeader = myLeader;
        RunAway = runAway;
    }

    public void OnEnter()
    {
        _myLeader.State = LeaderState.Run;
        _myLeader.UpdateColor();
        _myLeader.ResetCalc();
        _myLeader.FSMCalcPathToPosition(_myLeader.BaseNode.transform.position);
    }

    public void OnUpdate()
    {
        RunAway();
    }

    public void OnExit()
    {

    }
}
