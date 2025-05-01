using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HunterRest : IState
{
    FSM _fsm;
    Hunter _myHunter;
    SpriteRenderer _renderer;
    float _restTime;

    Action<IEnumerator> StartCoroutine;

    Action<Vector3> AddForce;
    Func<Vector3, Vector3> Seek;
    Func<Vector3, bool> FovCheck;

    Boid closeBoid = null;


    public HunterRest(FSM fsm, Hunter myHunter, SpriteRenderer render,  float time, Action<IEnumerator> newStart, Action<Vector3> newAddForce, Func<Vector3,Vector3> newSeek, Func<Vector3, bool> fovCheck) 
    {
        _fsm = fsm;
        _myHunter = myHunter;
        _renderer = render;
        _restTime = time;
        StartCoroutine = newStart;

        AddForce = newAddForce;
        Seek = newSeek;
        FovCheck = fovCheck;
    }

    public void OnEnter()
    {
        _renderer.color = Color.black;
        //_myHunter.hunterState = AgentState.Rest;

        StartCoroutine(Resting(_restTime));
    }

    IEnumerator Resting(float time)
    {
        yield return new WaitForSeconds(time);

        _myHunter.RestoreEnergy();

        bool startHunt = false;
        closeBoid = null;
        foreach (var boid in GameManager.Instance.allBoids)
        {
            if (FovCheck(boid.transform.position))
            {
                //Debug.Log($"yo soy {_myHunter}, vi a {boid}");
                startHunt = true;
                if (closeBoid == null) closeBoid = boid;
                if (Vector3.Distance(_myHunter.transform.position, boid.transform.position) < Vector3.Distance(_myHunter.transform.position, closeBoid.transform.position))
                    closeBoid = boid;
            }

        }
        if (startHunt)
        {
            _myHunter.closeBoid = closeBoid;
            _fsm.ChangeState(AgentState.Hunt);
        }
        else
            _fsm.ChangeState(AgentState.Patrol);
    }

    public void OnExit()
    {
        _renderer.color = Color.blue;
    }

    public void OnUpdate()
    {
        //throw new System.NotImplementedException();
        AddForce(Seek(_myHunter.transform.position));
    }

}
