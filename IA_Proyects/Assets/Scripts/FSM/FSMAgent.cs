using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMAgent : MonoBehaviour
{
    FSM _fsm;
    [SerializeField] SpriteRenderer _spriteR;
    [SerializeField] float _speed = 4f;


    private void Awake()
    {
        _fsm = new FSM();

        //add Idle
        _fsm.AddState(AgentState.Idle, new IdleState(_fsm, _spriteR));
        //add Movement
        _fsm.AddState(AgentState.Movement, new MovementState(_fsm, transform, _speed));

        //Default Idle
        _fsm.ChangeState(AgentState.Idle);
    }

    private void Update()
    {
        _fsm.FakeUpdate();
    }
}

public enum AgentState
{
    Idle,
    Movement
}
