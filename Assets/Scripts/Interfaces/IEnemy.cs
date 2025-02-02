using UnityEngine;

public interface IEnemy
{
    bool IsPlayerPetrolArea { get; set; }
    bool IsPlayerDetected { get; set; }
    bool ChangingPatrolPoint { get; set; }
    
    bool IsPlatformEnemy { get; set; }
    bool IsStunned { get; set; }
    Transform[] PatrolPoints { get; set; }
    float PatrolSpeed { get; set; }
    float ChaseSpeed { get; set; }
    float AttackRange { get; set; }
    float AttackCooldown { get; set; }
    int Health { get; set; }
    CapsuleCollider2D CapsuleCollider { get; set; }
    BoxCollider2D enemyCollider { get; set; }

    void TakeDamage(int damage);
    void Attack();
    void Stun();
}