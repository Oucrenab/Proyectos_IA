using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGameManager : MonoBehaviour
{
    public static PathfindingGameManager instance;

    Pathfinding _myPath;
    public Node startNode;
    public Node goalNode;
    public bool canMove;
    //public NPC myAgent;
    public TypeOfPathfinding myPathType;
    public TypeOfPathCalc myPathCalc;

    public event Action OnResetActivated;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        _myPath = new Pathfinding();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && goalNode != null && startNode != null)
        {
            canMove = false;
            OnResetActivated();

            switch (myPathType)
            {
                case TypeOfPathfinding.BFS:
                    StartCoroutine(_myPath.CalculateBFSCoroutine(startNode, goalNode));
                    break;
                case TypeOfPathfinding.Dijkstra:
                    StartCoroutine(_myPath.CalculateDijkstraCoroutine(startNode, goalNode)); 
                    break;
                case TypeOfPathfinding.GreedyBFS:
                    StartCoroutine(_myPath.CalculateGreedyBFSCoroutine(startNode, goalNode));
                    break;
                case TypeOfPathfinding.AStar:
                    StartCoroutine(_myPath.CalculateAStarCoroutine(startNode, goalNode));
                    break;
                case TypeOfPathfinding.ThetaStar:
                    break;
            }
        }

        if(Input.GetKeyDown(KeyCode.P) && canMove == true)
        {
            switch (myPathType)
            {
                case TypeOfPathfinding.BFS:
                    //myAgent.SetMove(_myPath.CalculateBFS(startNode, goalNode));
                    break;
                case TypeOfPathfinding.Dijkstra:
                    //myAgent.SetMove(_myPath.CalculateDijkstra(startNode, goalNode));
                    break;
                case TypeOfPathfinding.GreedyBFS:
                    //myAgent.SetMove(_myPath.CalculateGreedyBFS(startNode, goalNode));
                    break;
                case TypeOfPathfinding.AStar:
                    //myAgent.SetMove(_myPath.CalculateAStar(startNode, goalNode));
                    break;
                case TypeOfPathfinding.ThetaStar:
                    break;
            }
        }
    }
}

public enum TypeOfPathfinding
{
    BFS, Dijkstra, GreedyBFS, AStar, ThetaStar
}

public enum TypeOfPathCalc
{
    Euclidean, Manhattan
}
