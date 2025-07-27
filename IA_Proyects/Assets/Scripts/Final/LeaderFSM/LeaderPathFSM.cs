using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderPathFSM : IState
{
    LeaderFSM _fsm;
    Leader _myLeader;
    Action FollowPath;

    public LeaderPathFSM(LeaderFSM fsm, Leader myLeader, Action followPath)
    {
        _fsm = fsm;
        _myLeader = myLeader;
        FollowPath = followPath;
    }

    public void OnEnter()
    {
        if(_myLeader.State == LeaderState.Run)
            _fsm.ChangeState(_myLeader.State);
        _myLeader.State = LeaderState.GoingToPoint;
        _myLeader.UpdateColor();


    }

    public void OnUpdate()
    {
        FollowPath();
    }

    public void OnExit()
    {

    }
}
