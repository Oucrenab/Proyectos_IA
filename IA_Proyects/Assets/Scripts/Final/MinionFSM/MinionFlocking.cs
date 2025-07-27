using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionFlocking : IState
{
    MinionFSM _fsm;
    Minion _myMinion;
    Action Flocking;

    public MinionFlocking(MinionFSM fsm, Minion myMinion, Action flocking)
    {
        _fsm = fsm;
        _myMinion = myMinion;
        Flocking = flocking;
    }

    public void OnEnter()
    {
        _myMinion.State = MinionState.Flocking;
        _myMinion.UpdateColor();

    }

    public void OnUpdate()
    {
        Flocking();
    }

    public void OnExit()
    {

    }
}
