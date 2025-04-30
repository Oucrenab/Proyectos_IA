using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Agent
{
    [SerializeField] Transform _food;
    //[SerializeField] float _arriveRad;

    protected override void Update()
    {
        AddForce(Arrive(_food.transform.position));

        base.Update();
    }

    
}
