using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Agent
{
    [SerializeField] Transform[] _wayPoints;
    [SerializeField, Range(0,1f)] float _changePontDist;
    int _currentWayPoint;

    protected override void Update()
    {
        AddForce(Seek(_wayPoints[_currentWayPoint].position));

        if (Vector3.Distance(_wayPoints[_currentWayPoint].position, transform.position) < _changePontDist)
        {
            _currentWayPoint++;
            if(_currentWayPoint >= _wayPoints.Length) _currentWayPoint = 0; 
        }

        base.Update();
    }
}
