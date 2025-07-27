using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Boid, IDamageable
{
    [SerializeField] Leader _myLeader;
    [SerializeField] Team _team;
    public Team GetTeam() => _team;

    [SerializeField, Range(0f, 1f)] public float _separationForce;
    [SerializeField, Range(0f, 1f)] public float _cohesionForce;
    [SerializeField, Range(0f, 1f)] public float _alignmetForce;
    public float separationRadius;
    public float cohesionAlignmentRadius;

    [SerializeField] List<Agent> enemiesList = new List<Agent>();
    [SerializeField] List<Agent> myBoids = new List<Agent>();

    Pathfinding _myPath;

    [SerializeField] TypeOfPathCalc _myCalc;
    [SerializeField] List<Node> _currentPath;
    [SerializeField] Node _startNode;
    [SerializeField] Node _baseNode;
    public Vector3 _lastDesiredPos;

    bool _canCalculatePath = true;

    [SerializeField] MinionFSM _fsm;
    [SerializeField] MinionState _state = MinionState.PathToLeader;
    public MinionState State {  get { return _state; } set { _state = value; } }

    //FeedBack
    [SerializeField] Color _flockColor;
    [SerializeField] Color _followPathColor;
    [SerializeField] Color _attackColor;
    [SerializeField] Color _runAwayColor;
    [SerializeField] SpriteRenderer _renderer;

    protected override void Awake()
    {
        base.Awake();

        _myPath = new Pathfinding().SetObstacleMask(_obstacle);
        _life = _maxLife;
        _fsm = new MinionFSM();
        _fsm.AddState(MinionState.PathToLeader, new MinionsPathToLeader(_fsm, this, PathUpdate));
        _fsm.AddState(MinionState.RunAway, new MinionRunAway(_fsm, this, RunAwayUpdate));
        _fsm.AddState(MinionState.Flocking, new MinionFlocking(_fsm, this, FlockUpdate));
        _fsm.AddState(MinionState.Attack, new MinionAttack(_fsm, this, AttackUpdate));

        _fsm.ChangeState(_state);

    }

    protected override void Start()
    {
        //base.Start();
        GetClossestNode();
        //Debug.Log(_startNode.name);
    }

    protected override void Update()
    {
        //PathUpdate();
        //FlockUpdate();
        //if(_life == 0)
        //    _state = MinionState.RunAway;
        //else if(enemiesList.Count > 0)
        //    _state = MinionState.Attack;
        //else if (!InLOS(transform.position, _myLeader.transform.position))
        //    _state = MinionState.PathToLeader;
        //else
        //    _state = MinionState.Flocking;


        //switch (_state)
        //{
        //    case MinionState.PathToLeader:
        //        PathUpdate();
        //        break;
        //    case MinionState.Flocking:
        //        FlockUpdate();
        //        break;
        //    case MinionState.Attack:
        //        AttackUpdate();
        //        break;
        //    case MinionState.RunAway:
        //        RunAwayUpdate();
        //        break;
        //}

        _fsm.FakeUpdate();
        Debug.DrawRay(transform.position, _velocity, Color.green);

        //base.Update();
        //Debug.DrawRay(transform.position, _velocity, Color.red);

        
    }

    void FlockUpdate()
    {

        if (_life == 0)
        {
            _state = MinionState.RunAway;
        }
        else if (enemiesList.Count > 0)
        {
            _state = MinionState.Attack;

        }
        else if (!InLOS(transform.position, _myLeader.transform.position))
        {
            _state = MinionState.PathToLeader;

        }

        if (_state != MinionState.Flocking)
        {
            //Debug.Log($"FlockUpdate entro a {_state}");

            _fsm.ChangeState(_state);
        }


        if (Vector3.Distance(transform.position, _myLeader.transform.position) > .3f)
        AddForce(Arrive(_myLeader.transform.position));
            //_velocity = Vector3.zero;

        CheckForAgents(out enemiesList, out myBoids);
        myBoids.Add(_myLeader);

        Flocking();
        AddForce(ObstacleAvoidance());
        //AddForce(Separation(myBoids, separationRadius) * _separationForce);

        //_errasedNodes = 0;
        //_canCalculatePath = true;
        transform.position = transform.position + _velocity * Time.deltaTime;
        transform.right = _velocity;
    }

    void PathUpdate()
    {
        //Debug.DrawLine(transform.position, _myLeader.transform.position, Color.green);
        //Debug.DrawRay(transform.position, _velocity, Color.red);

        //si veo a lider flocking
        if (_life == 0)
        {
            _state = MinionState.RunAway;
        }
        else if (enemiesList.Count > 0)
        {
            _state = MinionState.Attack;

        }
        else if (InLOS(transform.position, _myLeader.transform.position))
        {
            _state = MinionState.Flocking;

        }

        if (_state != MinionState.PathToLeader)
        {
            //Debug.Log($"PathUpdate entro a {_state}");
            _fsm.ChangeState(_state);
        }


        FSMCalcPathToPosition(_myLeader.transform.position);
        //GetClossestNode();

        GoToPosition();
        CheckForAgents(out enemiesList, out myBoids);
        //Flocking();
        //AddForce(Separation(myBoids, separationRadius) * _separationForce);
        AddForce(ObstacleAvoidance());
        //AddForce(Cohesion(myBoids, cohesionAlignmentRadius) * _cohesionForce);
        //AddForce(Alignment(myBoids, cohesionAlignmentRadius) * _alignmetForce);
        transform.position = transform.position + _velocity * Time.deltaTime;
        transform.right = _velocity;
    }

    [SerializeField] float _attackCD;
    [SerializeField] int _attackDmg;
    [SerializeField] Bullet _bulletPrefab;

    float _lastAttack;

    void AttackUpdate()
    {
        CheckForAgents(out enemiesList, out myBoids);

        if (_life == 0)
        {
            _state = MinionState.RunAway;
        }
        else if (enemiesList.Count <= 0)
        {
            _state = MinionState.PathToLeader;

        }

        if(_state != MinionState.Attack)
        {
            //Debug.Log($"AttackUpdate entro a {_state}");

            _fsm.ChangeState(_state);
        }

        _velocity = Vector3.zero;

        if (Time.time > _lastAttack + _attackCD)
            Attack();

        //transform.position = transform.position + _velocity * Time.deltaTime;
        //transform.right = _velocity;
    }

    void RunAwayUpdate()
    {
        if (_life > 0)
        {
            _state = MinionState.PathToLeader;
        }

        if(_state != MinionState.RunAway)
        {
            Debug.Log($"RunUpdate entro a {_state}");

            _fsm.ChangeState(_state);
        }

        FSMCalcPathToPosition(_baseNode.transform.position);
        //GetClossestNode();

        GoToPosition();
        CheckForAgents(out enemiesList, out myBoids);
        //Flocking();
        //AddForce(Separation(myBoids, separationRadius) * _separationForce);
        AddForce(ObstacleAvoidance());
        transform.position = transform.position + _velocity * Time.deltaTime;
        transform.right = _velocity;
    }


    void Attack()
    {
        var bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity)
            .SetDamage(_attackDmg)
            .SetTeam(_team);
        bullet.transform.up = ChooseAttackDir(enemiesList);

        _lastAttack = Time.time;
    }

    Vector3 ChooseAttackDir(List<Agent> targets)
    {

        float lastDist = -1;
        Vector3 finalTarget = Vector3.zero;
        for (int i = 0; i < targets.Count; i++)
        {
            if(lastDist < 0) finalTarget = targets[i].transform.position;

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

    void CheckForAgents(out List<Agent> enemyList, out List<Agent> boids)
    {
        var posibleAgents = Physics2D.CircleCastAll(transform.position, _viewRange, transform.forward);


        List<Agent> allEnemies = new List<Agent>();
        List<Agent> allayBoids = new List<Agent>();

        foreach (var agent in posibleAgents)
        {
            //Debug.Log(agent.transform.name);
            if(agent.transform.TryGetComponent<Minion>(out var minion))
            {
                if (minion.GetTeam() == _team)
                    allayBoids.Add(minion);
                else if (minion.State != MinionState.RunAway)
                    allEnemies.Add(minion);

                continue;
            }
            if (agent.transform.TryGetComponent<Leader>(out var leader) && leader.GetTeam() != _team && leader.State != LeaderState.Run)
                allEnemies.Add(leader);
        }
        //allayBoids.Add(_myLeader);

        enemyList = EnemyInFov(allEnemies);
        boids = allayBoids;
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

    void GoToPosition()
    {
        //if (_lastDesiredPos == null) return;
        //if (InFOV(_lastDesiredPos)) _currentPath.Clear();
        if (InLOS(transform.position, _lastDesiredPos))
        {
            ResetCalc();
            _currentPath.Clear();
        }
        //if (!(_currentPath.Count > 0))
        if (!(_currentPath.Count > 0))
        {
            //if (InFOV(_lastPlayerPos))
            AddForce(Arrive(_lastDesiredPos));
            Flocking();
            AddForce(ObstacleAvoidance());
            //Debug.Log("ARRIVE");

            if (Vector3.Distance(transform.position, _lastDesiredPos) < 0.25f)
            {
                //Debug.Log("Llegue a la ultima posicion de player");
                _velocity = Vector3.zero;
                //Debug.Log("ZERO");

                //FSMCalcPathToPatrol();
            }

            //if(!_canCalculatePath) _canCalculatePath = true;

            return;
        }

        FollowPath();
    }


    //PathFinig Here XD
    #region PathFinding

    #region comment
    //void GoToPosition()
    //{
    //    //if (_lastDesiredPos == null) return;
    //    //if (InFOV(_lastDesiredPos)) _currentPath.Clear();
    //    if (InLOS(transform.position, _lastDesiredPos)) _currentPath.Clear();
    //    //if (!(_currentPath.Count > 0))
    //    if (!(_currentPath.Count > 0))
    //    {
    //        //if (InFOV(_lastPlayerPos))
    //        AddForce(Arrive(_lastDesiredPos));
    //        AddForce(ObstacleAvoidance());
    //        //Debug.Log("ARRIVE");

    //        if (Vector3.Distance(transform.position, _lastDesiredPos) < 0.25f)
    //        {
    //            //Debug.Log("Llegue a la ultima posicion de player");
    //            _velocity = Vector3.zero;
    //            //Debug.Log("ZERO");

    //            //FSMCalcPathToPatrol();
    //        }

    //        //if(!_canCalculatePath) _canCalculatePath = true;

    //        return;
    //    }

    //    FollowPath();
    //} 
    #endregion
    void FollowPath()
    {
        if (!(_currentPath.Count > 0))
        {
            ResetCalc();
            return;
        }
        //Debug.Log("AHHHHHHH");

        //AddForce(Arrive(_currentPath[0].transform.position));
        AddForce(Seek(_currentPath[0].transform.position));
        //Debug.DrawLine(transform.position, transform.position+_velocity, Color.yellow);
        //Debug.DrawLine(transform.position, _currentPath[0].transform.position, Color.blue);

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

    int _errasedNodes;

    public void ResetCalc()
    {
        _errasedNodes = 0;
        _canCalculatePath = true;
    }

    public void CalcPathToLeader() => FSMCalcPathToPosition(_myLeader.transform.position);
    public void CalcPathToBase() => FSMCalcPathToPosition(_baseNode.transform.position);

    void FSMCalcPathToPosition(Vector3 position)
    {
        //if (_currentState == EnemyState.ChasePlayer) return;
        if (!_canCalculatePath) return;
        _canCalculatePath = false;


        _lastDesiredPos = position;

        var _closeNode = FindCloseNodeToPos(_lastDesiredPos);
        if (_closeNode == null)
        {
            //FSMCalcPathToPatrol();
            Debug.Log("<color=red> No hay nodo cerca de desired position</color>");
            Invoke("ResetCalc", .2f);
            //_lastDesiredPos = transform.position;
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

    public void GetClossestNode()
    {
        var posibleNeightbors = Physics2D.OverlapCircleAll(transform.position, _viewRange * 3f);

        foreach (var node in posibleNeightbors)
        {
            //Debug.Log(node);
            if (!InLOS(transform.position, node.transform.position)) continue;

            if (!node.TryGetComponent<Node>(out var nodo)) continue;

            if (_startNode == null) _startNode = node.GetComponent<Node>();

            if (Vector2.Distance(transform.position, node.transform.position)
                > Vector2.Distance(transform.position, _startNode.transform.position)) continue;

            _startNode = node.GetComponent<Node>();

        }

        //Debug.Log($"{name} start node {_startNode}");
    }

    #endregion


    protected override void Flocking()
    {
        AddForce(Separation(myBoids, separationRadius) * (_separationForce * _maxForce));
        AddForce(Cohesion(myBoids, cohesionAlignmentRadius) * (_cohesionForce * _maxForce));
        AddForce(Alignment(myBoids, cohesionAlignmentRadius) * (_alignmetForce * _maxForce));
    }

    protected override Vector3 Seek(Vector3 desired)
    {
        desired -= transform.position;
        desired.Normalize();

        return base.Seek(desired);
    }

    protected override Vector3 Arrive(Vector3 target)
    {
        float dist = Vector3.Distance(target, transform.position);

        if (dist > _arriveRad)
            return Seek(target);
        else
        {
            var desired = (target - transform.position).normalized;

            desired *= _maxVelocity * (dist / _arriveRad);

            var steering = desired - _velocity;
            steering.z = 0;

            steering = Vector3.ClampMagnitude(steering, _maxForce);

            return steering;
        }
    }

    protected override Vector3 Separation(List<Agent> myBoids, float radius)
    {
        Vector3 desired = Vector3.zero;


        foreach (var boid in myBoids)
        {
            if (boid == this) continue;

            var dir = boid.transform.position - transform.position;
            if (dir.magnitude > radius) continue;

            //if (dir.magnitude < radius * 0.2f)
            //    desired -= dir * 100;
            //else
                desired -= dir;
        }

        if (desired == Vector3.zero) return Vector3.zero;

        return base.Seek(desired);
        //return base.Separation(myBoids, radius);
    }

    protected override Vector3 Cohesion(List<Agent> myBoids, float radius)
    {
        Vector3 desired = transform.position;
        int count = 0;


        foreach (var boid in myBoids)
        {
            if (boid == this) continue;
            if(boid == _myLeader)
            {
                count++;
                desired += boid.transform.position;
                continue;
            }

            if (Vector3.Distance(transform.position, boid.transform.position) > radius)
                continue;

            count++;
            desired += boid.transform.position;
        }

        if (count == 0)
            return Vector3.zero;

        desired /= count;

        desired -= transform.position;

        //desired.Normalize();

        return base.Seek(desired);
    }

    protected override Vector3 Alignment(List<Agent> myBoids, float radius)
    {
        Vector3 desired = Vector3.zero;
        int count = 0;


        foreach (var boid in myBoids)
        {
            if (boid == this) continue;
            if (boid == _myLeader)
            {
                count++;
                desired += boid.transform.position;
                continue;
            }
            if (Vector3.Distance(transform.position, boid.transform.position) > radius)
                continue;

            count++;
            desired += boid.Velocity;
        }

        if (count == 0)
            return Vector3.zero;

        desired /= count;
        //desired.Normalize();

        return base.Seek(desired);
    }

    protected override Vector3 ObstacleAvoidance()
    {
        return base.ObstacleAvoidance();
    }

    [SerializeField] int _maxLife;
    int _life;

    public void GetDamage(int amount)
    {
        //Debug.Log($"{name} Damaged {amount}");
        _life -= amount;

        if (_life <= 0)
        {
            //Debug.Log($"{name} Murio");
            _life = 0;
        }

        if (_life > _maxLife)
        {
            //Debug.Log($"{name} Curado");
            _life = _maxLife;
        }
    }

    protected override bool InLOS(Vector3 start, Vector3 end)
    {
        if (Vector3.Distance(start, end) > _viewRange)
            return false;
        else
            return base.InLOS(start, end);
    }

    public void UpdateColor()
    {
        switch (_state)
        {
            case MinionState.PathToLeader:
                _renderer.color = _followPathColor;
                break;
            case MinionState.Flocking:
                _renderer.color = _flockColor;
                break;
            case MinionState.Attack:
                _renderer.color = _attackColor;
                break;
            case MinionState.RunAway:
                _renderer.color = _runAwayColor;
                break;
        }
    }
}

public enum MinionState
{
    PathToLeader,
    Flocking,
    Attack,
    RunAway
}
