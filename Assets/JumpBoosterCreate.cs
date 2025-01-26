using System;
using UnityEngine;

public class JumpBoosterCreate : MonoBehaviour, IEnemy
{
    public bool isPlatformEnemy;
    public bool IsPlayerPetrolArea { get; set; }
    public bool IsPlayerDetected { get; set; }
    public bool ChangingPatrolPoint { get; set; }

    public bool IsPlatformEnemy
    {
        get => isPlatformEnemy;
        set => isPlatformEnemy = value;
    }

    public bool IsStunned { get; set; }
    public Transform[] PatrolPoints { get; set; }
    public float PatrolSpeed { get; set; }
    public float ChaseSpeed { get; set; }
    public float AttackRange { get; set; }
    public float AttackCooldown { get; set; }
    public int Health { get; set; }
    public CapsuleCollider2D CapsuleCollider { get; set; }
    public BoxCollider2D enemyCollider { get; set; }

    private void Start()
    {
        IsStunned = true;
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Stun()
    {
        throw new System.NotImplementedException();
    }
}