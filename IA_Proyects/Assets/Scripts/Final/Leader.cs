using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : Agent ,IDamageable
{
    //estados
    //Nada
    //LLendo a punto
    //Atacar
    //Huir
    [Header("Leader")]
    [SerializeField] Team _myTeam;
    Pathfinding _myPath;
    [SerializeField] TypeOfPathCalc _myCalc;
    public Team GetTeam() => _myTeam;

    [SerializeField] List<Node> _currentPath;
    [SerializeField] Node _startNode;
    [SerializeField] public Node BaseNode;
    public Vector3 _lastDesiredPos;

    [SerializeField] LeaderFSM _fsm;
    [SerializeField] LeaderState _state = LeaderState.Idle;
    public LeaderState State { get { return _state; } set { _state = value; } }

    bool _canCalculatePath = true;


    //View
    [SerializeField] SpriteRenderer _renderer;
    [SerializeField] Color _idleColor;
    [SerializeField] Color _attackColor;
    [SerializeField] Color _moveColor;
    [SerializeField] Color _runColor;


    protected override void Awake()
    {
        base.Awake();

        _myPath = new Pathfinding().SetObstacleMask(_obstacle);
        _myPath.myPathCalc = _myCalc;

        _life = _maxLife;

        _fsm = new LeaderFSM();
        _fsm.AddState(LeaderState.Idle, new LeaderIdleFSM(_fsm, this, IdleUpdate));
        _fsm.AddState(LeaderState.Attack, new LeaderAttackFSM(_fsm, this, AttackUpdate));
        _fsm.AddState(LeaderState.Run, new LeaderRunFSM(_fsm, this, RunAwayUpdate));
        _fsm.AddState(LeaderState.GoingToPoint, new LeaderPathFSM(_fsm, this, PathUpdate));

        _fsm.ChangeState(_state);

        //_lastDesiredPos = transform.position;
    }
    protected override void Update()
    {
        //GoToPosition();
        //FollowPath();
        //AddForce(Seek(TEST_Pos.position));

        //if (Input.GetKeyDown(KeyCode.Space))
        //    FSMCalcPathToPosition(TEST_Pos.position);

        _fsm.FakeUpdate();

        Debug.DrawRay(transform.position, _velocity, Color.blue);
    }

    void IdleUpdate()
    {
        CheckForEnemies(out enemiesList);

        if(_life <= 0)
            _state = LeaderState.Run;
        else if (enemiesList.Count > 0)
            _state = LeaderState.Attack;

        if(_state != LeaderState.Idle)
        {
            _fsm.ChangeState(_state);
            return;
        }



        _velocity = Vector3.zero;

        //transform.position = transform.position + _velocity * Time.deltaTime;
        //transform.right = _velocity;
    }

    [SerializeField] float _attackCD;
    [SerializeField] int _attackDmg;
    float _lastAttack;
    List<Agent> enemiesList = new();

    void AttackUpdate()
    {
        CheckForEnemies(out enemiesList);

        if (_life <= 0)
            _state = LeaderState.Run;
        else if (enemiesList.Count <= 0)
            _state = LeaderState.Idle;

        if (_state != LeaderState.Attack)
        {
            _fsm.ChangeState(_state);
            return;
        }


        if (_life == 0)
        {
            _state = LeaderState.Run;
        }


        if (_state != LeaderState.Attack)
        {
            Debug.Log($"AttackUpdate entro a {_state}");

            _fsm.ChangeState(_state);
        }

        _velocity = Vector3.zero;

        if (Time.time > _lastAttack + _attackCD)
            Attack();

        //transform.position = transform.position + _velocity * Time.deltaTime;
        //transform.right = _velocity;
    }

    [SerializeField] Bullet _bulletPrefab;

    void Attack()
    {
        var bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity)
            .SetDamage(_attackDmg)
            .SetTeam(_myTeam);
        bullet.transform.up = ChooseAttackDir(enemiesList);

        _lastAttack = Time.time;
    }

    void CheckForEnemies(out List<Agent> enemyList)
    {
        var posibleAgents = Physics2D.CircleCastAll(transform.position, _viewRange, transform.forward);


        List<Agent> allEnemies = new List<Agent>();

        foreach (var agent in posibleAgents)
        {
            //Debug.Log(agent.transform.name);
            if (agent.transform.TryGetComponent<Minion>(out var minion) && minion.GetTeam() != _myTeam && minion.State != MinionState.RunAway)
                allEnemies.Add(minion);

            if (agent.transform.TryGetComponent<Leader>(out var leader) && leader.GetTeam() != _myTeam && leader.State != LeaderState.Run)
                allEnemies.Add(leader);
        }

        enemyList = EnemyInFov(allEnemies);
    }

    List<Agent> EnemyInFov(List<Agent> list)
    {
        var finalList = new List<Agent>();
        for (int i = 0; i < list.Count; i++)
        {
            if (InFOV(list[i].transform.position))
                finalList.Add(list[i]);
        }
        return finalList;
    }

    Vector3 ChooseAttackDir(List<Agent> targets)
    {

        float lastDist = -1;
        Vector3 finalTarget = Vector3.zero;
        for (int i = 0; i < targets.Count; i++)
        {
            if (lastDist < 0) finalTarget = targets[i].transform.position;

            var dist = Vector3.Distance(transform.position, targets[i].transform.position);
            if (dist < lastDist)
            {
                lastDist = dist;
                finalTarget = targets[i].transform.position;
            }
        }

        finalTarget = finalTarget - transform.position;

        return finalTarget.normalized;
    }

    void PathUpdate()
    {

        CheckForEnemies(out enemiesList);

        if (_life <= 0)
            _state = LeaderState.Run;
        else if (enemiesList.Count > 0)
            _state = LeaderState.Attack;

        if (_state != LeaderState.GoingToPoint)
        {
            _fsm.ChangeState(_state);
            return;
        }

        GoToPosition();

        transform.position = transform.position + _velocity * Time.deltaTime;
        transform.right = _velocity;
    }

    void RunAwayUpdate()
    {
        if (_life > 0)
            _state = LeaderState.Idle;
        

        if (_state == LeaderState.Idle)
        {
            _fsm.ChangeState(_state);
            return;
        }

        GoToPosition();

        transform.position = transform.position + _velocity * Time.deltaTime;
        transform.right = _velocity;
    }

    public Vector3 pos;

    public void SetTargetPos(Vector3 pos)
    {
        ResetCalc();
        FSMCalcPathToPosition(pos);
        //tate = LeaderState.GoingToPoint;
        _fsm.ChangeState(LeaderState.GoingToPoint);
    }

    void FollowPath()
    {
        if (!(_currentPath.Count > 0)) return;
        //Debug.Log("AHHHHHHH");

        AddForce(Seek(_currentPath[0].transform.position));

        if (Vector3.Distance(_currentPath[0].transform.position, transform.position) < 0.25f)
        {
            _currentPath.RemoveAt(0);
            _errasedNodes++;
            if (_errasedNodes >= 2)
            {
                ResetCalc();
            }
        }
    }

    void GoToPosition()
    {
        //if (_lastDesiredPos == null) return;
        //if (InFOV(_lastDesiredPos)) _currentPath.Clear();
        if (InLOS(transform.position , _lastDesiredPos)) _currentPath.Clear();
        //if (!(_currentPath.Count > 0))
        if (!(_currentPath.Count > 0))
        {
            //if (InFOV(_lastPlayerPos))
            AddForce(Arrive(_lastDesiredPos));
            AddForce(ObstacleAvoidance());
            //Debug.Log("ARRIVE");

            if (Vector3.Distance(transform.position, _lastDesiredPos) < 0.25f)
            {
                //Debug.Log("Llegue a la ultima posicion de player");
                _state = LeaderState.Idle;
                //Debug.Log("ZERO");

                //FSMCalcPathToPatrol();
            }

            //if(!_canCalculatePath) _canCalculatePath = true;

            return;
        }

        FollowPath();
    }

    int _errasedNodes;

    public void ResetCalc()
    {
        _errasedNodes = 0;
        _canCalculatePath = true;
    }

    public void FSMCalcPathToPosition(params object[] position)
    {
        //if (_currentState == EnemyState.ChasePlayer) return;
        if (!_canCalculatePath) return;
        _canCalculatePath = false;

        _lastDesiredPos = (Vector3)position[0];

        var _closeNode = FindCloseNodeToPos(_lastDesiredPos);
        if (_closeNode == null)
        {
            //FSMCalcPathToPatrol();
            Debug.Log("<color=red> No hay nodo cerca de desired position</color>");
            _lastDesiredPos = transform.position;
            return;
        }

        GetClossestNode();

        _currentPath = _myPath.CalculateThetaStar(_startNode, _closeNode);
        //_lastPyDetectTime = Time.time;
        //_currentState = EnemyState.GoToPlayer;
        //_fsm.ChangeState(EnemyState.GoToPlayer);
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

    void GetClossestNode()
    {
        var posibleNeightbors = Physics2D.OverlapCircleAll(transform.position, _viewRange * 3f);

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

    [SerializeField] int _maxLife;
    int _life;

    public void GetDamage(int amount)
    {
        _life -= amount;

        if(_life <= 0)
        {
            Debug.Log($"{name} Murio");
            _life = 0;
        }

        if (_life > _maxLife)
        {
            Debug.Log($"{name} Curado");
            _life = _maxLife;
        }
    }

    public void UpdateColor()
    {
        switch (_state)
        {
            case LeaderState.GoingToPoint:
                _renderer.color = _moveColor;
                break;
            case LeaderState.Attack:
                _renderer.color = _attackColor;
                break;
            case LeaderState.Run:
                _renderer.color = _runColor;
                break;
            case LeaderState.Idle:
                _renderer.color = _idleColor;
                break;
        }
    }
}

public enum LeaderState
{
    GoingToPoint,
    Attack,
    Run,
    Idle
}

public enum Team
{
    Red,
    Blue,
    None
}
