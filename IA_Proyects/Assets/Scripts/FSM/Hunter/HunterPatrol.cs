using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HunterPatrol : IState
{
    FSM _fsm;
    Hunter _myHunter;
    SpriteRenderer _renderer;
    Transform[] _wayPoints;
    float _changePointDistance;

    Action<Vector3> AddForce;
    Func<Vector3, Vector3> Seek;
    Func<Vector3, bool> FovCheck;

    int _currentWayPoint;


    public HunterPatrol(FSM fsm, Hunter myHunter,SpriteRenderer render, Transform[] wayPoint, float changeDist, Action<Vector3> newAddForce, Func<Vector3, Vector3> newSeek, Func<Vector3, bool> fovCheck)
    {
        _fsm = fsm;
        _myHunter = myHunter;
        _renderer = render;
        _wayPoints = wayPoint;
        _changePointDistance = changeDist;
        AddForce = newAddForce;
        Seek = newSeek;
        FovCheck = fovCheck;
    }


    public void OnEnter()
    {
        _renderer.color = Color.blue;
        //_myHunter.hunterState = AgentState.Patrol;
    }

    public void OnExit()
    {
        _renderer.color = Color.red;
    }

    public void OnUpdate()
    {
        AddForce(Seek(_wayPoints[_currentWayPoint].position));

        if (Vector3.Distance(_wayPoints[_currentWayPoint].position, _myHunter.transform.position) < _changePointDistance)
        {
            _currentWayPoint++;
            if (_currentWayPoint >= _wayPoints.Length) _currentWayPoint = 0;
        }

        bool startHunt = false;
        Boid closeBoid = null;
        foreach(var boid in GameManager.Instance.allBoids)
        {
            if (FovCheck(boid.transform.position))
            {
                //Debug.Log($"yo soy {_myHunter}, vi a {boid}");
                startHunt = true;
                if(closeBoid == null) closeBoid = boid;
                if (Vector3.Distance(_myHunter.transform.position, boid.transform.position) < Vector3.Distance(_myHunter.transform.position, closeBoid.transform.position))
                    closeBoid = boid;
            }

        }
        if(startHunt)
        {
            _myHunter.closeBoid = closeBoid;
            _fsm.ChangeState(AgentState.Hunt);
        }
    }



}
