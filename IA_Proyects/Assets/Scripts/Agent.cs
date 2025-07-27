using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agent : FOVAgent
{
    protected Agent _target;
    [Space]
    [Header("<color=green>Agent Stats</color>")]
    [SerializeField, Range(0, 20)] protected float _maxVelocity;
    [SerializeField, Range(0, 10f)] protected float _maxForce;
    [SerializeField, Range(0f, 2f)] protected float _sphereCastRadius = 1;
    [SerializeField, Range(0f, 1f)] protected float _maxOAForce;
    [SerializeField] protected float _arriveRad;
    [Space]
    protected SteeringElection _currentElection;

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
        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);
        
        
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

    protected virtual Vector3 Arrive(Vector3 target)
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

    protected virtual Vector3 ObstacleAvoidance()
    {
        Vector3 pos = transform.position;
        Vector3 dir = transform.right;
        //Vector3 dir = _velocity;
        float dist = _velocity.magnitude;

        //if (Physics.SphereCast(pos, _sphereCastRadius, dir, out RaycastHit hit, dist, _obstacle))
        var hit = Physics2D.CircleCast(pos, _sphereCastRadius, dir, _sphereCastRadius, _obstacle);
        if (hit && hit.transform.lossyScale.sqrMagnitude < transform.lossyScale.sqrMagnitude * 2)
        {
            var obstacle = hit.transform;
            Vector3 dirToObj = obstacle.position - pos;
            float angle = Vector3.SignedAngle(dir, dirToObj, Vector3.up);
            Vector3 desired = angle > 0 ? -transform.up : transform.up;
            desired = desired.normalized;
            desired *= _maxVelocity;

            //Steering
            Vector3 steering = desired - _velocity;
            return Vector3.ClampMagnitude(steering, _maxOAForce * _maxForce);
        }
        return Vector3.zero;
    }
}
