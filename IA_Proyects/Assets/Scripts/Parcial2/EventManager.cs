using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventManager
{
    public delegate void MyEvents(params object[] parameters);

    static Dictionary<string, MyEvents> _events = new Dictionary<string, MyEvents>();

    public static void Subscribe(string name, MyEvents method)
    {
        if (_events.ContainsKey(name))
            _events[name] += method;
        else
            _events.Add(name, method);
    }
    public static void Unsubscribe(string name, MyEvents method)
    {
        if (_events.ContainsKey(name))
        {
            _events[name] -= method;

            if (_events[name] == null)
                _events.Remove(name);
        }
    }
    public static void Trigger(string name, params object[] parameters)
    {
        if (_events.ContainsKey(name))
            _events[name](parameters);
    }
}
