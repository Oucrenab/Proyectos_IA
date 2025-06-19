using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVAgent : FOVTarget
{
    //[SerializeField]protected List<FOVTarget> _otherAgents;

    [Header("<color=blue>FOV Agent</color>")]
    [SerializeField]protected LayerMask _obstacle;
    [Space]
    [SerializeField, Range(0, 360)] protected float _maxViewAngle;
    [SerializeField, Range(0, 360)] protected float _viewAngle;
    [SerializeField, Range(0.5f, 15)]protected float _viewRange;

    protected override void Awake()
    {
        _viewAngle = _maxViewAngle;
    }

    protected virtual void Start()
    {
        //ChangeColor(Color.red);
    }

    protected virtual void Update()
    {


        //foreach (var agent in _otherAgents)
        //{
        //    agent.ChangeColor(InFOV(agent.transform.position) ? Color.blue : Color.white);
        //}
    }

    protected bool InFOV(Vector3 endPos)
    {
        var dir = endPos - transform.position;
        if (dir.magnitude > _viewRange) return false;
        if (Vector3.Angle(transform.right, dir) > _viewAngle * 0.5f) return false;
        if (!InLOS(transform.position, endPos)) return false;


        return true;
    }

    protected bool InLOS(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;

        return !(Physics2D.Raycast(start, dir.normalized, dir.magnitude, _obstacle));
    }

    protected bool InLOS(Vector3 start, Vector3 end, float radius)
    {
        Vector3 dir = end - start;

        return !(Physics2D.Raycast(start, dir.normalized, radius, _obstacle));
    }


    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRange);

        Vector3 dirA = GetAngleFromDir(_viewAngle * 0.5f + transform.eulerAngles.z);
        Vector3 dirB = GetAngleFromDir(-_viewAngle * 0.5f + transform.eulerAngles.z);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + dirA.normalized * _viewRange);
        Gizmos.DrawLine(transform.position, transform.position + dirB.normalized * _viewRange);
    }

    Vector3 GetAngleFromDir(float angleIndeg) => new Vector3(Mathf.Cos(angleIndeg * Mathf.Deg2Rad), Mathf.Sin(angleIndeg * Mathf.Deg2Rad), 0);
}
