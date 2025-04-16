using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{//a
    public static GameManager Instance;

    public float width, height;

    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector3 GetPosition(Vector3 position)
    {
        if(position.x <-width) position.x = width;
        if(position.x >width) position.x = -width;
        if(position.y <-height) position.x = height;
        if(position.y >height) position.x = -height;

        return position;
    }

    private void OnDrawGizmos()
    {
        Vector3 LeftDown = (Vector3.right * -width) + (Vector3.up * -height);
        Vector3 RightDown = (Vector3.right * width) + (Vector3.up * -height);
        Vector3 LeftUp = (Vector3.right * -width) + (Vector3.up * height);
        Vector3 RightUp = (Vector3.right * width) + (Vector3.up * height);


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(LeftDown, LeftUp);
        Gizmos.DrawLine(RightUp, LeftUp);
        Gizmos.DrawLine(RightUp, RightDown);
        Gizmos.DrawLine(LeftDown, RightDown);
    }
}
