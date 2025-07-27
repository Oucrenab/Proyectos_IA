using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderAttackFSM : IState
{
    LeaderFSM _fsm;
    Leader _myLeader;
    Action Attack;

    public LeaderAttackFSM(LeaderFSM fsm, Leader myLeader, Action attack)
    {
        _fsm = fsm;
        _myLeader = myLeader;
        Attack = attack;
    }

    public void OnEnter()
    {
        _myLeader.State = LeaderState.Attack;
        _myLeader.UpdateColor();

    }

    public void OnUpdate()
    {
        Attack();
    }

    public void OnExit()
    {

    }
}
