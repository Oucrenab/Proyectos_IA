using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Agent
{

    [Header("<color=red> Hunter </color>")]
    [SerializeField] FSM _fsm;
    [Space]
    [SerializeField] Transform[] _wayPoints;
    [SerializeField, Range(0,1f)] float _changePontDist;
    [Space]
    [SerializeField, Range(0, 10f)] float _restTime;
    [Space]
    [SerializeField] protected SpriteRenderer _spriteR;
    [Space]
    int _currentWayPoint;

    public Boid closeBoid;

    public float ViewAngle
    {
        get { return _viewAngle; }
        set { _viewAngle = value; }
    }
    //public AgentState hunterState;

    protected override void Awake()
    {
        base.Awake();
        _fsm = new FSM();

        _spriteR = GetComponent<SpriteRenderer>();

        _fsm.AddState(AgentState.Patrol, new HunterPatrol(_fsm, this, _spriteR, _wayPoints, _changePontDist, AddForce, Seek, InFOV));
        _fsm.AddState(AgentState.Hunt, new HunterHunt(_fsm, this, _spriteR,AddForce,Persuit,InFOV));
        _fsm.AddState(AgentState.Rest, new HunterRest(_fsm,this, _spriteR, _restTime ,CallStartCoroutine, AddForce, Seek,InFOV));

        _fsm.ChangeState(AgentState.Patrol);
    }

    protected override void Start()
    {
        //base.Start();

        GameManager.Instance.allHunter.Add(this);

        //StartCoroutine(CheckForBoids());
        
    }

    protected override void Update()
    {
        _fsm.FakeUpdate();

        #region Comment
        //AddForce(Seek(_wayPoints[_currentWayPoint].position));

        //if (Vector3.Distance(_wayPoints[_currentWayPoint].position, transform.position) < _changePontDist)
        //{
        //    _currentWayPoint++;
        //    if(_currentWayPoint >= _wayPoints.Length) _currentWayPoint = 0; 
        //} 
        #endregion

        

        base.Update();
    }

    //IEnumerator CheckForBoids()
    //{
    //    while(hunterState == AgentState.Patrol)
    //    {
    //        bool startHunt = false;
    //        foreach (var boid in GameManager.Instance.allBoids)
    //        {
    //            if (InFOV(boid.transform.position))
    //            {
    //                startHunt = true;
    //                if (closeBoid == null) closeBoid = boid;
    //                if (closeBoid != boid)
    //                {
    //                    if (Vector3.Distance(transform.position, boid.transform.position) < Vector3.Distance(transform.position, closeBoid.transform.position))
    //                        closeBoid = boid;
    //                }
    //            }
    //        }

    //        if (startHunt) _fsm.ChangeState(AgentState.Hunt);

    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    void CallStartCoroutine(IEnumerator routine)
    {
        StartCoroutine(routine);
    }

    private void OnDestroy()
    {
        GameManager.Instance.allHunter.Remove(this);
    }
    //a
    [SerializeField, Range(0,100)] float _energy;
    [SerializeField,Range(1, 20)] float _consumeRate;
    public float Energy { get { return _energy; } }

    public void ConsumeEnergy(float amount)
    {
        _energy -= amount * _consumeRate;
    }

    public void RestoreEnergy()
    {
        _energy = 100;
    }

    public void ResetViewAngle()
    {
        _viewAngle = _maxViewAngle;
    }
}

public enum AgentState
{
    Patrol,
    Hunt,
    Rest
}
