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

    public HunterRest(FSM fsm, Hunter myHunter, SpriteRenderer render,  float time, Action<IEnumerator> newStart, Action<Vector3> newAddForce, Func<Vector3,Vector3> newSeek) 
    {
        _fsm = fsm;
        _myHunter = myHunter;
        _renderer = render;
        _restTime = time;
        StartCoroutine = newStart;

        AddForce = newAddForce;
        Seek = newSeek;
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
