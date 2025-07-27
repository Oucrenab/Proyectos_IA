using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class Node : FOVAgent
{
    Grid _myGrid;
    [Header("<color=green>BFS Node</color>")]
    [SerializeField] Material _myBaseMaterial;
    [SerializeField] Material _myBlockMaterial;
    [SerializeField] Material _myStartMaterial;
    [SerializeField] Material _myGoalMaterial;
    public MeshRenderer myMesh;
    int _xPos, _yPos;
    [SerializeField] List<Node> _neighbors = new List<Node>();
    public bool Block { get; private set; }
    public int Cost { get; private set; }
    [SerializeField] TextMeshProUGUI _costText;

    protected override void Start()
    {
        Debug.Log("Nodo Start");
        _neighbors.Clear();
        _neighbors = SetNeightbors();
    }

    public void Initialize(Grid myGrid, int xPos, int yPos)
    {
        _myGrid = myGrid;
        _xPos = xPos;
        _yPos = yPos;
        myMesh = GetComponent<MeshRenderer>();
        Cost = 1;
        _costText.text = Cost.ToString();

        PathfindingGameManager.instance.OnResetActivated += ResetNode;
    }

    void ResetNode()
    {
        if (Block) return;

        if (PathfindingGameManager.instance.startNode == this) ChangeColor(_myStartMaterial.color);
        else if (PathfindingGameManager.instance.goalNode == this) ChangeColor(_myGoalMaterial.color);
        else ChangeColor(_myBaseMaterial.color);
    }

    //public void ChangeColor(Color color)
    //{
    //    myMesh.material.color = color;
    //}

    public List<Node> GetNeighbors
    {
        get
        {
            if (_neighbors.Count > 0) return _neighbors;

            //var nodeUp = _myGrid.GetNode(_xPos, _yPos + 1);
            Node nodeUp = null;
            if (nodeUp != null) _neighbors.Add(nodeUp);

            Node nodeRight = null;
            if (nodeRight != null) _neighbors.Add(nodeRight);

            Node nodeDown = null;
            if (nodeDown != null) _neighbors.Add(nodeDown);

            Node nodeLeft = null;
            if (nodeLeft != null) _neighbors.Add(nodeLeft);

            return _neighbors;
        }
    }

    List<Node> SetNeightbors()
    {
        var posibleNeightbors = Physics2D.OverlapCircleAll(transform.position, _viewRange);
        //var posibleNeightbors = Physics.OverlapSphere(transform.position, _viewRange);

        List<Node> finalNeightbors = new List<Node>();

        foreach(var neighbor in posibleNeightbors)
        {
            if(!neighbor.TryGetComponent<Node>(out var node) || node == this) continue;

            if (InLOS(transform.position, neighbor.transform.position)) /*continue;*/
                finalNeightbors.Add(neighbor.GetComponent<Node>());
        }

        return finalNeightbors;
    }

    //private void OnMouseOver()
    //{
    //    if (Input.GetMouseButtonDown(2)) //Rueda del medio
    //    {
    //        Block = !Block;

    //        myMesh.material = Block ? _myBlockMaterial : _myBaseMaterial;
    //    }

    //    if (Input.GetMouseButtonUp(0)) //Click Izquierdo
    //    {
    //        if (PathfindingGameManager.instance.startNode != null)
    //            PathfindingGameManager.instance.startNode.myMesh.material = _myBaseMaterial;

    //        PathfindingGameManager.instance.startNode = this;
    //        myMesh.material = _myStartMaterial;
    //    }

    //    if (Input.GetMouseButtonUp(1)) //Click Derecho
    //    {
    //        if (PathfindingGameManager.instance.goalNode != null)
    //            PathfindingGameManager.instance.goalNode.myMesh.material = _myBaseMaterial;

    //        PathfindingGameManager.instance.goalNode = this;
    //        myMesh.material = _myGoalMaterial;
    //    }

    //    if (Input.mouseScrollDelta.y > 0)
    //    {
    //        SetCost(+1);
    //    }

    //    if (Input.mouseScrollDelta.y < 0)
    //    {
    //        SetCost(-1);
    //    }
    //}

    void SetCost(int value)
    {
        Cost = Mathf.Clamp(Cost + value, 1, int.MaxValue);
        _costText.text = Cost.ToString();
    }
}
