using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementState : IState
{
    FSM _fsm;
    Transform _target;
    float _speed;

    public MovementState( FSM fsm , Transform target, float speed)
    {
        _fsm = fsm;
        _target = target;
        _speed = speed;
    }

    public void OnEnter()
    {
        Debug.Log("Como la mueve esa muchachota");
    }

    public void OnExit()
    {
        Debug.Log("Ya no la mueve");
    }

    public void OnUpdate()
    {
        _target.position += _target.right * _speed * Time.deltaTime;

        //if (Input.GetKeyUp(KeyCode.W))
            //_fsm.ChangeState(OldAgentState.Idle);
    }
}
