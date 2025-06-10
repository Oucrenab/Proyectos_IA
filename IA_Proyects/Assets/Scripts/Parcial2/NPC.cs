using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] float _speed = 5;
  
    public void SetMove(List<Node> nodes)
    {
        transform.position = new Vector3(nodes[0].transform.position.x, nodes[0].transform.position.y, nodes[0].transform.position.z - 1);
        StartCoroutine(StartMoving(nodes));
    }

    IEnumerator StartMoving(List<Node> nodes)
    {
        while (nodes.Count > 0)
        {
            var dir = new Vector3(nodes[0].transform.position.x, nodes[0].transform.position.y, nodes[0].transform.position.z - 1) - transform.position;

            transform.position += dir.normalized * _speed * Time.deltaTime;

            if(dir.magnitude < 0.25f) nodes.RemoveAt(0);
            
            yield return new WaitForEndOfFrame();
        }
    }
}
