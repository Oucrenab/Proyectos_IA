using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionRunAway : IState
{
    MinionFSM _fsm;
    Minion _myMinion;
    Action RunAway;

    public MinionRunAway(MinionFSM fsm, Minion myMinion, Action runAway)
    {
        _fsm = fsm;
        _myMinion = myMinion;
        RunAway = runAway;
    }

    public void OnEnter()
    {
        //Debug.Log($"{_myMinion.name} en RunAway");
        _myMinion.State = MinionState.RunAway;
        _myMinion.UpdateColor();


        _myMinion.ResetCalc();
        _myMinion.GetClossestNode();
        _myMinion.CalcPathToBase();
    }

    public void OnUpdate()
    {
        RunAway();
    }

    public void OnExit()
    {

    }
}
