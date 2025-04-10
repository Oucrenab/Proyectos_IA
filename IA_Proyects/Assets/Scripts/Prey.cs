using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prey : Agent
{
    [SerializeField] Transform _food;
    [SerializeField] float _arriveRad;

    protected override void Update()
    {
        AddForce(Arrive(_food.transform.position));

        base.Update();
    }

    Vector3 Arrive(Vector3 target)
    {
        float dist = Vector3.Distance(target, transform.position);

        if (dist > _arriveRad) 
            return Seek(target);
        else
        {
            var desired = (target - transform.position).normalized;

            desired *= _maxVelocity * (dist / _arriveRad);

            var steering = desired - _velocity;
            steering = Vector3.ClampMagnitude(steering, _maxForce);

            return steering;
        }
    }
}
