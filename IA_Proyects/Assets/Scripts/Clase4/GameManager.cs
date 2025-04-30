using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{//a
    public static GameManager Instance;

    [Header("<color=yellow>Layout Size</color>")]
    public float width;
    public float height;
    [Range(0, 1f)] public float tpOffset;
    [Space]
    [Header("<color=green>Boids variaables</color>")]
    [SerializeField, Range(0f, 1f)] public float separationForce;
    [SerializeField, Range(0f, 1f)] public float cohesionForce;
    [SerializeField, Range(0f, 1f)] public float alignmetForce;

    public float separationRadius;
    public float cohesionAlignmentRadius;

    public List<Boid> allBoids = new();
    public List<Food> allFood = new();
    public List<Hunter> allHunter = new();


    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Vector3 GetPosition(Vector3 position)
    {
        if(position.x <-width) position.x = width - tpOffset;
        if(position.x >width) position.x = -width + tpOffset;
        if(position.y <-height) position.y = height - tpOffset;
        if(position.y >height) position.y = -height + tpOffset;

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
