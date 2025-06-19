using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Enemy : Agent
{
    Pathfinding _myPath;

    [Header("<color=red>ENEMY</color>")]
    [SerializeField] Node _goalNode;
    [SerializeField] Node _startNode;
    [SerializeField] TypeOfPathCalc _myCalc;
    [Space]
    [SerializeField] Node[] _patrolNodes;
    [SerializeField] List<Node> _currentPath;
    [Space]
    [SerializeField] public Player player;
    [Space]
    [SerializeField] EnemyState _currentState;
    [SerializeField] bool _canCalculatePath;
    [SerializeField] float _calcCD;
    [Space]
    [SerializeField] float _hearArea;
    [SerializeField] EnemyFSM _fsm;

    int _currentNodeIndex = 0;

    float _lastPyDetectTime;
    Vector3 _lastPlayerPos;


    protected override void Awake()
    {
        base.Awake();
        _myPath = new Pathfinding();
        _myPath.myPathCalc = _myCalc;

        _fsm = new EnemyFSM();
        _fsm.AddState(EnemyState.Patrol, new EnemyPatrol(_fsm, this, Patrol));
        _fsm.AddState(EnemyState.GoToPatrol, new EnemyGoPatrol(_fsm,this, GoToPatrol));
        _fsm.AddState(EnemyState.ChasePlayer, new EnemyChase(_fsm,this, ChasePlayer));
        _fsm.AddState(EnemyState.GoToPlayer, new EnemyGoPlayer(_fsm, this, GoToPlayer));

        _fsm.ChangeState(EnemyState.Patrol);
    }

    protected override void Start()
    {
        base.Start();
        //EventManager.Subscribe("OnPlayerDetected", CalcPathToPlayer);
        EventManager.Subscribe("OnPlayerDetected", FSMCalcPathToPlayer);
    }

    protected override void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventManager.Trigger("OnPlayerDetected", player.transform.position);
        }

        //switch (_currentState)
        //{
        //    case EnemyState.Patrol:
        //        Patrol();
        //        break;
        //    case EnemyState.ChasePlayer:
        //        ChasePlayer();
        //        break;
        //    case EnemyState.GoToPlayer:
        //        FollowPlayerPath();
        //        break;
        //    case EnemyState.GoToPatrol:
        //        BackToPatrol();
        //        break;
        //    case EnemyState.None:
        //        _velocity = Vector3.zero;
        //        break;

        //}

        _fsm.FakeUpdate();

        if(Time.time > _lastPyDetectTime + _calcCD && _canCalculatePath == false)
            _canCalculatePath = true;
        

        transform.position = transform.position + _velocity * Time.deltaTime;
        transform.right = _velocity;
    }


    public bool InFov(Vector3 end) => InFOV(end);

    public bool ToClose()
    {
        if (!InLOS(transform.position, player.transform.position)) return false;

        //Debug.Log($"Player a {Vector2.Distance(transform.position, _player.transform.position)}");
        if(Vector2.Distance(transform.position, player.transform.position) > _hearArea) return false;

        return true;
    }

    void ChasePlayer()
    {
        AddForce(Arrive(player.transform.position));
    }

    void GoToPlayer()
    {
        if (InFOV(_lastPlayerPos)) _currentPath.Clear();
        //if (!(_currentPath.Count > 0))
        if (!(_currentPath.Count > 0))
        {
            //if (InFOV(_lastPlayerPos))
            AddForce(Arrive(_lastPlayerPos));

            if (Vector3.Distance(transform.position, _lastPlayerPos) < 0.25f)
            {
                //Debug.Log("Llegue a la ultima posicion de player");
                _velocity = Vector3.zero;

                FSMCalcPathToPatrol();
            }

            //if(!_canCalculatePath) _canCalculatePath = true;

            return;
        }

        FollowPath();
    }

    void Patrol()
    {
        //var dir = new Vector3(nodes[0].transform.position.x, nodes[0].transform.position.y, nodes[0].transform.position.z - 1) - transform.position;

        //transform.position += dir.normalized * _speed * Time.deltaTime;

        //if (dir.magnitude < 0.25f) nodes.RemoveAt(0);

        //Debug.Log("MOVIENDOME");

        AddForce(Seek(_patrolNodes[_currentNodeIndex].transform.position));

        if (Vector3.Distance(_patrolNodes[_currentNodeIndex].transform.position, transform.position) < 0.25f)
        {
            _currentNodeIndex++;
            if (_currentNodeIndex >= _patrolNodes.Length) _currentNodeIndex = 0;
        }

        //if(!_canCalculatePath) _canCalculatePath=true;
    }

    void GoToPatrol()
    {
        if (!(_currentPath.Count > 0))
        {
            //Debug.Log("Llegue a la ultima posicion de Patrulla");
            _velocity = Vector3.zero;

            _fsm.ChangeState(EnemyState.Patrol);
            return;
        }

        FollowPath();
    }

    void FollowPath()
    {
        if (!(_currentPath.Count > 0)) return;

        AddForce(Seek(_currentPath[0].transform.position));

        if (Vector3.Distance(_currentPath[0].transform.position, transform.position) < 0.25f)
        {
            _currentPath.RemoveAt(0);
            //if (_currentNodeIndex >= _patrolNodes.Length) _currentNodeIndex = 0;
            if(_currentState == EnemyState.GoToPlayer) 
            {
                //_canCalculatePath = true;
            }
        }
    }

    void GetClossestNode()
    {
        var posibleNeightbors = Physics2D.OverlapCircleAll(transform.position, _viewRange * 1.5f);

        foreach (var node in posibleNeightbors)
        {
            if (!InLOS(transform.position, node.transform.position)) continue;

            if (!node.TryGetComponent<Node>(out var nodo)) continue;

            if (_startNode == null) _startNode = node.GetComponent<Node>();

            if (Vector2.Distance(transform.position, node.transform.position)
                > Vector2.Distance(transform.position, _startNode.transform.position)) continue;

            _startNode = node.GetComponent<Node>();

        }
    }

    Node FindCloseNodeToPos(Vector3 pos)
    {
        var posibleNeightbors = Physics2D.OverlapCircleAll(pos, 20);
        Node _closeNode = null;

        foreach (var node in posibleNeightbors)
        {
            if (!InLOS(pos, node.transform.position)) continue;

            if (!node.TryGetComponent<Node>(out var nodo)) continue;

            if (_closeNode == null) _closeNode = node.GetComponent<Node>();


            if (Vector2.Distance(pos, node.transform.position)
                > Vector2.Distance(pos, _closeNode.transform.position)) continue;

            _closeNode = node.GetComponent<Node>();

        }

        return _closeNode;
    }

    void FSMCalcPathToPlayer(params object[] playerPos)
    {
        if (_currentState == EnemyState.ChasePlayer) return;
        if (!_canCalculatePath) return;
        _canCalculatePath = false;

        _lastPlayerPos = (Vector3)playerPos[0];

        var _closeNode = FindCloseNodeToPos(_lastPlayerPos);
        if (_closeNode == null)
        {
            FSMCalcPathToPatrol();
            return;
        }

        GetClossestNode();

        _currentPath = _myPath.CalculateAStar(_startNode, _closeNode);
        _lastPyDetectTime = Time.time;
        _currentState = EnemyState.GoToPlayer;
        _fsm.ChangeState(EnemyState.GoToPlayer);
    }

    void FSMCalcPathToPatrol()
    {
        GetClossestNode();

        _currentPath = _myPath.CalculateAStar(_startNode, _patrolNodes[_currentNodeIndex]);

        _currentState = EnemyState.GoToPatrol;
        _fsm.ChangeState(EnemyState.GoToPatrol);
    }



    //Codigo Anterior al FSM, no es llamado o se llaman entre ellos

    #region Anterior a FSM
    void SearchPlayer()
    {

        if (InFOV(player.transform.position) || ToClose())
        {
            _currentState = EnemyState.ChasePlayer;

            //if(Time.time > _lastPyDetectTime + 2.5)
            EventManager.Trigger("OnPlayerDetected", player.transform.position);
            //_currentPath = _myPath.CalculateAStar(_startNode, _player.CloseNode);
        }
        else
        {
            if (_currentState == EnemyState.ChasePlayer)
            {
                _currentState = EnemyState.None;
                _canCalculatePath = true;
                CalcPathToPlayer(player.transform.position);
            }
        }
    }
    void FollowPlayerPath()
    {
        if (InFOV(_lastPlayerPos)) _currentPath.Clear();
        //if (!(_currentPath.Count > 0))
        if (!(_currentPath.Count > 0))
        {
            //if (InFOV(_lastPlayerPos))
            AddForce(Arrive(_lastPlayerPos));

            if (Vector3.Distance(transform.position, _lastPlayerPos) < 0.25f)
            {
                Debug.Log("Llegue a la ultima posicion de player");
                _velocity = Vector3.zero;

                CalcPathToPatrol();
            }

            //if(!_canCalculatePath) _canCalculatePath = true;

            return;
        }

        FollowPath();
    }
    void BackToPatrol()
    {
        if (!(_currentPath.Count > 0))
        {
            Debug.Log("Llegue a la ultima posicion de Patrulla");
            _velocity = Vector3.zero;

            _currentState = EnemyState.Patrol;
            return;
        }

        FollowPath();
    }
    void CalcPathToPlayer(params object[] playerPos)
    {
        if (_currentState == EnemyState.ChasePlayer) return;
        if (!_canCalculatePath) return;
        _canCalculatePath = false;

        _lastPlayerPos = (Vector3)playerPos[0];

        var _closeNode = FindCloseNodeToPos(_lastPlayerPos);
        if (_closeNode == null)
        {
            CalcPathToPatrol();
            return;
        }

        GetClossestNode();

        _currentPath = _myPath.CalculateAStar(_startNode, _closeNode);
        _lastPyDetectTime = Time.time;
        _currentState = EnemyState.GoToPlayer;
    }
    void CalcPathToPatrol()
    {
        GetClossestNode();

        _currentPath = _myPath.CalculateAStar(_startNode, _patrolNodes[_currentNodeIndex]);

        _currentState = EnemyState.GoToPatrol;
    } 
    #endregion

}
public enum EnemyState
{
    Patrol,
    ChasePlayer,
    GoToPlayer,
    GoToPatrol,
    None

}
