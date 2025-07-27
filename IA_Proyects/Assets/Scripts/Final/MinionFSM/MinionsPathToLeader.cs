using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionsPathToLeader : IState
{
    MinionFSM _fsm;
    Minion _myMinion;
    Action GoToLeader;

    public MinionsPathToLeader(MinionFSM fsm, Minion myMinion, Action goToLeader)
    {
        _fsm = fsm;
        _myMinion = myMinion;
        GoToLeader = goToLeader;
    }

    public void OnEnter()
    {
        //Debug.Log($"{_myMinion.name} en GoToLeader");
        _myMinion.State = MinionState.PathToLeader;
        _myMinion.UpdateColor();

        _myMinion.ResetCalc();
        _myMinion.GetClossestNode();
        _myMinion.CalcPathToLeader();
    }

    public void OnExit()
    {
        //throw new System.NotImplementedException();
        //Debug.Log($"{_myMinion.name} Salio GoToLeader");
    }

    public void OnUpdate()
    {
        GoToLeader();
    }
}
