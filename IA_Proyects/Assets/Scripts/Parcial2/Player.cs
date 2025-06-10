using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class Player : FOVAgent
{
    [Header("<color=green> PLAYER </color>")]
    [SerializeField] float _speed;
    [SerializeField] Node _closeNode;
    public Node CloseNode { get { return _closeNode; } }

    float _xInput;
    float _yInput;

    protected override void Update()
    {
        GetInputs();
        Movement();
    }

    private void FixedUpdate()
    {
        GetClossestNode();
    }

    void GetInputs()
    {
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(KeyCode.R))
            RestartScene();

    }

    void Movement()
    {
        var dir = new Vector3(_xInput, _yInput, 0).normalized;
        transform.position += dir * _speed * Time.deltaTime;
    }

    void RestartScene()
    {

    }

    void GetClossestNode()
    {
        var posibleNeightbors = Physics2D.OverlapCircleAll(transform.position, _viewRange);

        foreach(var node in posibleNeightbors)
        {
            if (!node.TryGetComponent<Node>(out var nodo)) continue;
            
            if(_closeNode == null) _closeNode = node.GetComponent<Node>();

            if(Vector2.Distance(transform.position, node.transform.position) 
                > Vector2.Distance(transform.position, _closeNode.transform.position)) continue;

            _closeNode = node.GetComponent<Node>();

        }
    }
}
