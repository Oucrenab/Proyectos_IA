using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGoPlayer : IState
{
    EnemyFSM _fsm;
    Enemy _myEnemy;
    Action GoToPlayer;

    public EnemyGoPlayer(EnemyFSM fsm, Enemy enemy, Action action)
    {
        _fsm = fsm;
        _myEnemy = enemy;
        GoToPlayer = action;
    }

    public void OnEnter()
    {
        //throw new System.NotImplementedException();
    }

    public void OnExit()
    {
        //throw new System.NotImplementedException();
    }

    public void OnUpdate()
    {
        GoToPlayer();

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
