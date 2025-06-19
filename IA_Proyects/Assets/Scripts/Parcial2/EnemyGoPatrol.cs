using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoPatrol : IState
{
    EnemyFSM _fsm;
    Enemy _myEnemy;
    Action GoToPatrol;

    public EnemyGoPatrol(EnemyFSM fsm, Enemy enemy, Action action)
    {
        _fsm = fsm;
        _myEnemy = enemy;
        GoToPatrol = action;
    }

    public void OnEnter()
    {


    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        GoToPatrol();

        SearchPlayer();
    }

    void SearchPlayer()
    {
        if (_myEnemy.InFov(_myEnemy.player.transform.position) || _myEnemy.ToClose())
        {
            _fsm.ChangeState(EnemyState.ChasePlayer);
        }
    }
}
