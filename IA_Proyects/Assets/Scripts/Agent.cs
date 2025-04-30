using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : FOVAgent
{
    [SerializeField] protected Agent _target;
    [Space]
    [Header("<color=green>Agent Stats</color>")]
    [SerializeField, Range(0, 20)] protected float _maxVelocity;
    [SerializeField, Range(0, 1)] protected float _maxForce;
    [SerializeField] float _arriveRad;
    [Space]
    [SerializeField] protected SteeringElection _currentElection;

    protected Vector3 _velocity;
    public Vector3 Velocity { get { return _velocity; } }
    protected override void Update()
    {
        #region Comment
        //switch (_currentElection)
        //{
        //    case SteeringElection.Seek:
        //        AddForce(Seek(_target.transform.position));
        //        break;
        //    case SteeringElection.Flee:
        //        AddForce(Flee(_target.transform.position));
        //        break;
        //    case SteeringElection.Persuit:
        //        AddForce(Persuit(_target));
        //        break;
        //    case SteeringElection.Evade:
        //        AddForce(Evade(_target));
        //        break;
        //} 
        #endregion

        //transform.position += _velocity * Time.deltaTime;
        transform.position = GameManager.Instance.GetPosition(transform.position + _velocity * Time.deltaTime);
        transform.right = _velocity;
    }

    protected virtual Vector3 Seek(Vector3 target)
    {
        var desired = (target - transform.position).normalized;
        
        desired *= _maxVelocity;
        
        var steering = desired - _velocity;
        steering.z = 0;
        steering = Vector3.ClampMagnitude(steering, _maxForce);
        
        
        return steering;
    }

    protected Vector3 Flee(Vector3 target) => -Seek(target);

    protected Vector3 Persuit(Agent target)
    {
        var desired = target.transform.position + target.Velocity;

        return Seek(desired);
    }

    protected Vector3 Evade(Agent target)
    {
        var desired = target.transform.position + target.Velocity;

        return Flee(desired);
    }

    protected Vector3 Arrive(Vector3 target)
    {
        float dist = Vector3.Distance(target, transform.position);

        if (dist > _arriveRad)
            return Seek(target);
        else
        {
            var desired = (target - transform.position).normalized;

            desired *= _maxVelocity * (dist / _arriveRad);

            var steering = desired - _velocity;
            steering.z = 0;

            steering = Vector3.ClampMagnitude(steering, _maxForce);

            return steering;
        }
    }

    protected void AddForce(Vector3 dir)
    {
        _velocity = Vector3.ClampMagnitude(_velocity + dir, _maxVelocity);

    }
}
