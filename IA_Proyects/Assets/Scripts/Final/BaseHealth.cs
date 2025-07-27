using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BaseHealth : MonoBehaviour
{
    [SerializeField] Team _team;
    [SerializeField] int _healthAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.TryGetComponent<IDamageable>(out var damageable) && damageable.GetTeam() == _team)
        {
            damageable.GetDamage(-_healthAmount);
        }
    }
}
