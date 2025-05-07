using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : Agent
{
    [SerializeField] public TreeNode _firtNode;

    protected Action MyMovement = delegate { };

    [SerializeField, Range(0,360)] float inEvadeViewRange;
    [Space]
    [SerializeField] BoidState _currentState;
    Agent _closeHunter = null;
    Food _closeFood = null;
    Vector3 _randomPos = Vector3.zero;

    [SerializeField]SpriteRenderer[] _spriteR;
    [SerializeField] Color _randomColor = Color.gray;
    [SerializeField] Color _flockingColor = Color.green;
    [SerializeField] Color _eatColor = Color.cyan;
    [SerializeField] Color _evadeColor = Color.yellow;

    protected override void Start()
    {
        GameManager.Instance.allBoids.Add(this);
        //AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0) * _maxVelocity);

        if (_firtNode != null)
            StartCoroutine(CallTree());
    }

    protected override void Update()
    {
        //Flocking();

        switch (_currentState)
        {
            case BoidState.EvadeHunter:
                if (_closeHunter == null)
                    AddForce(Evade(GameManager.Instance.allHunter[0]));
                else
                    AddForce(Evade(_closeHunter));
                
                break;
            case BoidState.ArribeFood:
                if (_closeFood != null)
                {
                    AddForce(Arrive(_closeFood.transform.position));


                    if (Vector3.Distance(_closeFood.transform.position, transform.position) < 0.1f)
                        _closeFood.GetEat();
                }

                break;
            case BoidState.Flocking:
                Flocking();

                break;
            case BoidState.RandomMovement:
                AddForce(base.Seek(_randomPos));

                break;

        }

        base.Update();
    }

    void Flocking()
    {
        //Debug.Log("Flockeando");

        AddForce(Separation(GameManager.Instance.allBoids, GameManager.Instance.separationRadius) * GameManager.Instance.separationForce);
        AddForce(Cohesion(GameManager.Instance.allBoids, GameManager.Instance.cohesionAlignmentRadius) * GameManager.Instance.cohesionForce);
        AddForce(Alignment(GameManager.Instance.allBoids, GameManager.Instance.cohesionAlignmentRadius) * GameManager.Instance.alignmetForce);
    }

    Vector3 Separation(List<Boid> myBoids, float radius)
    {
        Vector3 desired = Vector3.zero;


        foreach (Boid boid in myBoids)
        {
            if (boid == this) continue;

            var dir = boid.transform.position - transform.position;
            if (dir.magnitude > radius) continue;

            desired -= dir;
        }

        if (desired == Vector3.zero) return Vector3.zero;

        return Seek(desired);
    }

    Vector3 Cohesion(List<Boid> myBoids, float radius)
    {
        Vector3 desired = transform.position;
        int count = 0;


        foreach (Boid boid in myBoids)
        {
            if (boid == this) continue;
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

        return Seek(desired);
    }

    Vector3 Alignment(List<Boid> myBoids, float radius)
    {
        Vector3 desired = Vector3.zero;
        int count = 0;


        foreach (Boid boid in myBoids)
        {
            if (boid == this) continue;
            if (Vector3.Distance(transform.position, boid.transform.position) > radius)
                continue;

            count++;
            desired += boid.Velocity;
        }

        if (count == 0)
            return Vector3.zero;

        desired /= count;
        //desired.Normalize();

        return Seek(desired);
    }

    //Uso esto porque el Seek normal hace que loopeen
    protected override Vector3 Seek(Vector3 desired)
    {

        desired.Normalize();
        desired *= _maxVelocity;

        var steering = desired - _velocity;
        steering.z = 0;

        steering = Vector3.ClampMagnitude(steering, _maxForce * Time.deltaTime);

        return steering;
    }

    public bool CheckForHunter()
    {
        bool final = false;

        foreach (var hunter in GameManager.Instance.allHunter)
        {
            final = InFOV(hunter.transform.position);

            if(_closeHunter == null) _closeHunter = hunter;
            if(_closeHunter != hunter)
            {
                if (Vector3.Distance(transform.position, hunter.transform.position) < Vector3.Distance(transform.position, _closeHunter.transform.position))
                    _closeHunter = hunter;
            }
        }

        return final;
    }

    public void StartEvadeHunter()
    {
        foreach (var renderer in _spriteR)
            renderer.color = _evadeColor;

        _viewAngle = inEvadeViewRange;

        _currentState = BoidState.EvadeHunter;
    }

    public bool CheckForFood()
    {
        bool final = false;

        foreach (var food in GameManager.Instance.allFood)
        {
            if (!final)
                final = InFOV(food.transform.position);

            if (_closeFood == null) _closeFood = food;
            if (_closeFood != food)
            {
                if (Vector3.Distance(transform.position, food.transform.position) < Vector3.Distance(transform.position, _closeHunter.transform.position))
                    _closeFood = food;
            }
        }

        if (_closeFood != null) _closeFood.OnFoodDestroy += ResetFood;

        return final;
    }

    void ResetFood()
    {
        _closeFood = null;
    }

    public void StartFoodArribe()
    {
        if (_currentState == BoidState.ArribeFood) return;

        foreach (var renderer in _spriteR)
            renderer.color = _eatColor;

        _viewAngle = _maxViewAngle;

        _currentState = BoidState.ArribeFood;

    }

    public bool CheckForBoids()
    {

        bool final = false;

        foreach(var boid in GameManager.Instance.allBoids)
        {
            if (boid == this)
                continue;

            if (!final)
                final = InFOV(boid.transform.position);

        }

        return final;
    }

    public void StartFlocking()
    {
        if (_currentState == BoidState.Flocking) return;

        foreach (var renderer in _spriteR)
            renderer.color = _flockingColor;

        _viewAngle = _maxViewAngle;

        _currentState = BoidState.Flocking;

    }

    public void StartRandomMovement()
    {
        if(_currentState == BoidState.RandomMovement) return;
        _currentState = BoidState.RandomMovement;

        foreach (var renderer in _spriteR)
            renderer.color = _randomColor;

        _viewAngle = _maxViewAngle;

        StartCoroutine(ChangeRandomPos());

    }

    IEnumerator ChangeRandomPos()
    {
        while(_currentState == BoidState.RandomMovement)
        {
            _randomPos.x = UnityEngine.Random.Range(-GameManager.Instance.width, GameManager.Instance.width);
            _randomPos.y = UnityEngine.Random.Range(-GameManager.Instance.height, GameManager.Instance.height);
            _randomPos.z = 0;

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator CallTree()
    {
        while (true)
        {
            _firtNode.Execute(this);

            yield return new WaitForSeconds(0.1f);
        }
    }

    public void GetEaten()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.allBoids.Remove(this);
        foreach(var food in GameManager.Instance.allFood)
        {
            food.OnFoodDestroy -= ResetFood;
            food.OnFoodDestroy -= ResetFood;
            food.OnFoodDestroy -= ResetFood;
            food.OnFoodDestroy -= ResetFood;
            food.OnFoodDestroy -= ResetFood;
            food.OnFoodDestroy -= ResetFood;
            food.OnFoodDestroy -= ResetFood;
            food.OnFoodDestroy -= ResetFood;
        }
    }
}

enum BoidState
{
    EvadeHunter,
    ArribeFood,
    Flocking,
    RandomMovement
}
