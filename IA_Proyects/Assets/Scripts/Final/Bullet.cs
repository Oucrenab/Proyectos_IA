using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] float _lifeTime;
    [SerializeField] int _damage;
    [SerializeField] SpriteRenderer _renderer;
    Team _team = Team.None;
    
    float timer = 0f;

    private void Update()
    {
        Movement();



        timer += Time.deltaTime;
        if(timer > _lifeTime)
        {
            TurnOff();
        }
    }

    public Bullet SetTeam(Team team)
    {
        _team = team;

        switch (_team)
        {
            case Team.Red:
                _renderer.color = Color.yellow;
                break;
            case Team.Blue:
                _renderer.color = Color.cyan;
                break;
            case Team.None:
                _renderer.color = Color.white;
                break;
        }

        return this;
    }
    public Bullet SetDamage(int damage)
    {
        _damage = damage;
        return this;
    }

    void TurnOff()
    {
        Destroy(gameObject);
    }

    void Movement()
    {
        transform.position += transform.up * _speed * Time.deltaTime;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<IDamageable>(out var damageable) && damageable.GetTeam() != _team)
        {
            damageable.GetDamage(_damage);
            TurnOff();
        }
    }
}
