using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    [Space]
    [Header("<color=blue>Prefabs</color>")]
    [SerializeField] Boid boidPrefab;
    [SerializeField] Food foodPrefab;
    [Space]
    [SerializeField] Camera _camera;
    [SerializeField] TreeNode _firstNode;
    public List<Boid> allBoids = new();
    public List<Food> allFood = new();
    public List<Hunter> allHunter = new();


    private void Awake()
    {
        if(Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = GetMouseWordlPosition(_camera, _camera.transform.position.z);

            var boid = Instantiate(boidPrefab, mousePos, Quaternion.identity);
            boid._firtNode = _firstNode;
            boid.gameObject.SetActive(true);
            //Input.mousePosition
        }
        if(Input.GetMouseButtonDown(1))
        {
            var mousePos = GetMouseWordlPosition(_camera, _camera.transform.position.z);

            Instantiate(foodPrefab, mousePos, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(foodPrefab, new Vector3(UnityEngine.Random.Range(-width, width), UnityEngine.Random.Range(-height, height), 0), Quaternion.identity);

        }
    }



    Vector3 GetMouseWordlPosition(Camera cam, float worldDepth)
    {
        var screenPos = math.clamp(Input.mousePosition, Vector3.zero ,new Vector3(Screen.width -1 , Screen.height -1,0));
        screenPos.z = -worldDepth;
        return cam.ScreenToWorldPoint(screenPos);
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
