using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyPatrol : IState
{
    EnemyFSM _fsm;
    Enemy _myEnemy;
    Action Patrol;


    public EnemyPatrol(EnemyFSM fsm,Enemy enemy, Action newPatrol)
    {
        _fsm = fsm;
        _myEnemy = enemy;
        Patrol = newPatrol;
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
        Patrol();

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
