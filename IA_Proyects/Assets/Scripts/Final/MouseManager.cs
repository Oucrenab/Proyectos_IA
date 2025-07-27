using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField] Leader _blueLeader;
    [SerializeField] Leader _redLeader;

    Vector3 _bluePos;
    Vector3 _redPos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _bluePos = ClickPosition();
            _blueLeader.SetTargetPos(_bluePos);
        }
        if (Input.GetMouseButtonDown(1))
        {
            _redPos = ClickPosition();
            _redLeader.SetTargetPos(_redPos);
        }
    }

    Vector3 ClickPosition()
    {
        var _screenPosition = Input.mousePosition;

        var _worldPosition = Camera.main.ScreenToWorldPoint(_screenPosition);
        _worldPosition.z = 0;


        return _worldPosition;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_bluePos, 0.1f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_redPos, 0.1f);
    }


}
