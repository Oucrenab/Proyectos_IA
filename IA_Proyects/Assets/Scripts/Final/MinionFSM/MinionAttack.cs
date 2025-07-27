using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAttack : IState
{
    MinionFSM _fsm;
    Minion _myMinion;
    Action Attack;

    public MinionAttack(MinionFSM fsm, Minion myMinion, Action attack)
    {
        _fsm = fsm;
        _myMinion = myMinion;
        Attack = attack;
    }

    public void OnEnter()
    {
        _myMinion.State = MinionState.Attack;
        _myMinion.UpdateColor();

    }

    public void OnUpdate()
    {
        Attack();
    }

    public void OnExit()
    {

    }
}
