using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{

    IState _currentState;

    Dictionary<AgentState, IState> _allStates = new();

    public void AddState(AgentState newState, IState state)
    {

        if(_allStates.ContainsKey(newState)) return;

        _allStates.Add(newState, state);
    }

    public void ChangeState(AgentState newState)
    {
        if (_currentState != null) _currentState.OnExit();

        _currentState = _allStates[newState];
        _currentState?.OnEnter();
    }

    public void FakeUpdate()
    {
        if(_currentState != null) _currentState.OnUpdate();
    }

}
