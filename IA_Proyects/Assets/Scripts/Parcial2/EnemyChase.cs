using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : IState
{
    EnemyFSM _fsm;
    Enemy _myEnemy;
    Action Chase;

    public EnemyChase(EnemyFSM fsm, Enemy enemy, Action action)
    {
        _fsm = fsm;
        _myEnemy = enemy;
        Chase = action;
    }

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }

    public void OnUpdate()
    {
        Chase();

        SearchPlayer();
    }

    void SearchPlayer()
    {
        if ((_myEnemy.InFov(_myEnemy.player.transform.position) || _myEnemy.ToClose()))
        {
            EventManager.Trigger("OnPlayerDetected", _myEnemy.player.transform.position);
        }
        else
        {
            _fsm.ChangeState(EnemyState.GoToPlayer);
        }
    }
}
