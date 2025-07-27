using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Pathfinding
{
    LayerMask _obstacle;

    public Pathfinding SetObstacleMask(LayerMask obstacle)
    {
        _obstacle = obstacle;
        return this;
    }

    public List<Node> CalculateBFS(Node startNode, Node goalNode)
    {
        //frontier = Queue()
        var frontier = new Queue<Node>();

        //frontier.put(start)
        frontier.Enqueue(startNode);

        //came_from = dict() # path A->B is stored as came_from[B] == A
        var cameFrom = new Dictionary<Node, Node>();

        //while not frontier.empty():
        while (frontier.Count > 0)
        {
            //current = frontier.get()
            var current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }

                path.Add(current);
                path.Reverse();
                return path;
            }

            foreach (var node in current.GetNeighbors)
            {
                if (!node.Block && !cameFrom.ContainsKey(node))
                {
                    frontier.Enqueue(node);
                    cameFrom.Add(node, current);
                }
            }
        }
        return new List<Node>();
    }

    public List<Node> CalculateDijkstra(Node startNode, Node goalNode)
    {
        //frontier = Queue()
        var frontier = new PriorityQueue<Node>();

        //frontier.put(start)
        frontier.Enqueue(startNode, 0);

        //came_from = dict() # path A->B is stored as came_from[B] == A
        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, int>();

        costSoFar.Add(startNode, 0);

        //while not frontier.empty():
        while (frontier.Count > 0)
        {
            //current = frontier.get()
            var current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }

                path.Add(current);
                path.Reverse();
                return path;
            }

            foreach (var node in current.GetNeighbors)
            {
                if (node.Block) continue;

                int newCost = costSoFar[current] + node.Cost;

                if (!costSoFar.ContainsKey(node))
                {
                    costSoFar.Add(node, newCost);
                    frontier.Enqueue(node, newCost);
                    cameFrom.Add(node, current);
                }
                else if (costSoFar[node] > newCost)
                {
                    frontier.Enqueue(node, newCost);
                    cameFrom[node] = current;
                    costSoFar[node] = newCost;
                }
            }
        }
        return new List<Node>();
    }

    public List<Node> CalculateGreedyBFS(Node startNode, Node goalNode)
    {
        //frontier = Queue()
        var frontier = new PriorityQueue<Node>();

        //frontier.put(start)
        frontier.Enqueue(startNode, 0);

        //came_from = dict() # path A->B is stored as came_from[B] == A
        var cameFrom = new Dictionary<Node, Node>();

        //while not frontier.empty():
        while (frontier.Count > 0)
        {
            //current = frontier.get()
            var current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }

                path.Add(current);
                path.Reverse();
                return path;
            }

            foreach (var node in current.GetNeighbors)
            {
                if (!node.Block && !cameFrom.ContainsKey(node))
                {
                    int priority = Heuristic(node.transform.position, goalNode.transform.position);
                    frontier.Enqueue(node, priority);
                    cameFrom.Add(node, current);
                }
            }
        }
        return new List<Node>();
    }

    public List<Node> CalculateAStar(Node startNode, Node goalNode)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startNode, 0);
        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startNode, 0);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }

                path.Add(current);
                path.Reverse();
                return path;
            }

            foreach (var node in current.GetNeighbors)
            {
                if (node.Block) continue;

                int newCost = costSoFar[current] + node.Cost;
                int priority = newCost + Heuristic(node.transform.position, goalNode.transform.position);

                if (!costSoFar.ContainsKey(node))
                {
                    costSoFar.Add(node, newCost);
                    frontier.Enqueue(node, priority);
                    cameFrom.Add(node, current);
                }
                else if (costSoFar[node] > newCost)
                {
                    frontier.Enqueue(node, priority);
                    cameFrom[node] = current;
                    costSoFar[node] = newCost;
                }
            }
        }
        return new List<Node>();
    }

    public List<Node> CalculateThetaStar(Node startNode, Node goalNode)
    {
        var listNode = CalculateAStar(startNode, goalNode);

        int currentCount = 0;

        while (currentCount + 2 < listNode.Count)
        {
            if (InLOS(listNode[currentCount].transform.position, listNode[currentCount + 2].transform.position))
                listNode.RemoveAt(currentCount + 1);
            else
                currentCount++;
        }
        return listNode;
    }

    public bool InLOS(Vector3 start, Vector3 end)
    {
        Vector3 dir = end - start;

        Debug.DrawRay(start, dir, Color.red, dir.magnitude);

        return !Physics2D.Raycast(start, dir.normalized, dir.magnitude, _obstacle);
    }

    public IEnumerator CalculateBFSCoroutine(Node startNode, Node goalNode)
    {
        //frontier = Queue()
        var frontier = new Queue<Node>();

        //frontier.put(start)
        frontier.Enqueue(startNode);

        //came_from = dict() # path A->B is stored as came_from[B] == A
        var cameFrom = new Dictionary<Node, Node>();

        //while not frontier.empty():
        while (frontier.Count > 0)
        {
            //current = frontier.get()
            var current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startNode)
                {
                    path.Add(current);
                    current = cameFrom[current];

                    current.ChangeColor(Color.green);
                    yield return new WaitForSeconds(0.025f);
                }

                path.Add(current);
                path.Reverse();
                PathfindingGameManager.instance.canMove = true;
                break;
            }
            else
            {
                current.ChangeColor(Color.yellow);
                foreach (var node in current.GetNeighbors)
                {
                    if (!node.Block && !cameFrom.ContainsKey(node))
                    {
                        frontier.Enqueue(node);
                        cameFrom.Add(node, current);
                        node.ChangeColor(Color.blue);
                    }
                    yield return new WaitForSeconds(0.025f);
                }
            }
        }
    }

    public IEnumerator CalculateDijkstraCoroutine(Node startNode, Node goalNode)
    {
        //frontier = Queue()
        var frontier = new PriorityQueue<Node>();

        //frontier.put(start)
        frontier.Enqueue(startNode, 0);

        //came_from = dict() # path A->B is stored as came_from[B] == A
        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startNode, 0);

        //while not frontier.empty():
        while (frontier.Count > 0)
        {
            //current = frontier.get()
            var current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startNode)
                {
                    path.Add(current);
                    current = cameFrom[current];

                    current.ChangeColor(Color.green);
                    yield return new WaitForSeconds(0.025f);
                }

                path.Add(current);
                path.Reverse();
                PathfindingGameManager.instance.canMove = true;
                break;
            }
            else
            {
                current.ChangeColor(Color.yellow);
                foreach (var node in current.GetNeighbors)
                {
                    if (node.Block) continue;

                    int newCost = costSoFar[current] + node.Cost;

                    if (!costSoFar.ContainsKey(node))
                    {
                        costSoFar.Add(node, newCost);
                        frontier.Enqueue(node, newCost);
                        cameFrom.Add(node, current);
                        node.ChangeColor(Color.blue);
                    }
                    else if (costSoFar[node] > newCost)
                    {
                        frontier.Enqueue(node, newCost);
                        cameFrom[node] = current;
                        costSoFar[node] = newCost;
                        node.ChangeColor(Color.blue);
                    }
                    yield return new WaitForSeconds(0.025f);
                }
            }
        }
    }

    public IEnumerator CalculateGreedyBFSCoroutine(Node startNode, Node goalNode)
    {
        //frontier = Queue()
        var frontier = new PriorityQueue<Node>();

        //frontier.put(start)
        frontier.Enqueue(startNode, 0);

        //came_from = dict() # path A->B is stored as came_from[B] == A
        var cameFrom = new Dictionary<Node, Node>();

        //while not frontier.empty():
        while (frontier.Count > 0)
        {
            //current = frontier.get()
            var current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startNode)
                {
                    path.Add(current);
                    current = cameFrom[current];
                    current.ChangeColor(Color.green);
                    yield return new WaitForSeconds(0.025f);
                }

                path.Add(current);
                path.Reverse();
                PathfindingGameManager.instance.canMove = true;
                break;
            }
            else
            {
                current.ChangeColor(Color.yellow);
                foreach (var node in current.GetNeighbors)
                {
                    if (!node.Block && !cameFrom.ContainsKey(node))
                    {
                        int priority = Heuristic(node.transform.position, goalNode.transform.position);
                        frontier.Enqueue(node, priority);
                        cameFrom.Add(node, current);
                        node.ChangeColor(Color.blue);
                    }
                    yield return new WaitForSeconds(0.025f);
                }
            }
        }
    }

    public IEnumerator CalculateAStarCoroutine(Node startNode, Node goalNode)
    {
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(startNode, 0);

        var cameFrom = new Dictionary<Node, Node>();
        var costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(startNode, 0);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == goalNode)
            {
                List<Node> path = new List<Node>();

                while (current != startNode)
                {
                    path.Add(current);
                    current = cameFrom[current];

                    current.ChangeColor(Color.green);
                    yield return new WaitForSeconds(0.025f);
                }

                path.Add(current);
                path.Reverse();
                PathfindingGameManager.instance.canMove = true;
                break;
            }
            else
            {
                current.ChangeColor(Color.yellow);
                foreach (var node in current.GetNeighbors)
                {
                    if (node.Block) continue;

                    int newCost = costSoFar[current] + node.Cost;
                    int priority = newCost + Heuristic(node.transform.position, goalNode.transform.position);

                    if (!costSoFar.ContainsKey(node))
                    {
                        costSoFar.Add(node, newCost);
                        frontier.Enqueue(node, priority);
                        cameFrom.Add(node, current);
                        node.ChangeColor(Color.blue);
                    }
                    else if (costSoFar[node] > newCost)
                    {
                        frontier.Enqueue(node, priority);
                        cameFrom[node] = current;
                        costSoFar[node] = newCost;
                        node.ChangeColor(Color.blue);
                    }
                    yield return new WaitForSeconds(0.025f);
                }
            }
        }
    }

    public TypeOfPathCalc myPathCalc;

    int Heuristic(Vector3 node, Vector3 goalNode)
    {
        switch (myPathCalc)
        {
            case TypeOfPathCalc.Euclidean:
                return (int)Vector3.Distance(node, goalNode);
            case TypeOfPathCalc.Manhattan:
                return (int)(Mathf.Abs(goalNode.x - node.x) + Mathf.Abs(goalNode.y - node.y));
        }
        return 1;
    }
}
