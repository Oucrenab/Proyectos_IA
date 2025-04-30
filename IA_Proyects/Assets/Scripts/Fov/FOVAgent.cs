using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVAgent : FOVTarget
{
    [SerializeField]protected List<FOVTarget> _otherAgents;

    [SerializeField]protected LayerMask _obstacle;

    [SerializeField, Range(0, 360)] protected float _viewAngre;
    [SerializeField, Range(0.5f, 15)]protected float _viewRange;

    protected virtual void Start()
    {
        ChangeColor(Color.red);
    }

    protected virtual void Update()
    {


        foreach (var agent in _otherAgents)
        {
            agent.ChangeColor(InFOV(agent.transform.position) ? Color.blue : Color.white);
        }
    }

    protected bool InFOV(Vector3 endPos)
    {
        var dir = endPos - transform.position;
        if (dir.magnitude > _viewRange) return false;
        if (Vector3.Angle(transform.right, dir) > _viewAngre * 0.5f) return false;
        if (!InLOS(transform.position, endPos)) return false;


        return true;
    }

    protected bool InLOS(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;

        return !(Physics.Raycast(start, dir.normalized, dir.magnitude, _obstacle));
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, _viewRange);

        Vector3 dirA = GetAngleFromDir(_viewAngre * 0.5f + transform.eulerAngles.y);
        Vector3 dirB = GetAngleFromDir(-_viewAngre * 0.5f + transform.eulerAngles.y);

        Gizmos.DrawLine(transform.position, transform.position + dirA.normalized);
        Gizmos.DrawLine(transform.position, transform.position + dirB.normalized);
    }

    Vector3 GetAngleFromDir(float angleIndeg) => new Vector3(Mathf.Sin(angleIndeg * Mathf.Deg2Rad),0,Mathf.Cos(angleIndeg* Mathf.Deg2Rad));
}
