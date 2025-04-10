using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarlosBoton : MonoBehaviour
{

    [SerializeField] KeyCode _useKey;
    [SerializeField] float _offDelay;

    public event CarlosLogica.VoidDelegate OnButtonActive;

    private void Update()
    {
        if (Input.GetKeyDown(_useKey))
        {
            Activate();
        }
    }

    void Activate()
    {
        OnButtonActive();

        TurnOn();

    }
    
    public void TurnOn(float delay = 0)
    {


        if(delay < 0) delay = _offDelay;

        Invoke("TurnOff", delay);
    }

    public void TrollOn(float delay = 0)
    {
        if (delay < 0) delay = _offDelay;

        Invoke("TurnOff", delay);
    }

    void TurnOff()
    {

    }
}
