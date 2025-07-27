using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionFSM
{
    IState _currentState;

    Dictionary<MinionState, IState> _allStates = new();

    public void AddState(MinionState newState, IState state)
    {

        if (_allStates.ContainsKey(newState)) return;

        _allStates.Add(newState, state);
    }

    public void ChangeState(MinionState newState)
    {
        if (_currentState != null) _currentState.OnExit();

        _currentState = _allStates[newState];
        _currentState?.OnEnter();
    }

    public void FakeUpdate()
    {
        if (_currentState != null) _currentState.OnUpdate();
    }
}
