using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour, IEnemy
{
    public bool isPlayerPetrolArea;
    public bool isPlayerDetected;
    public bool changingPatrolPoint;
    public Transform[] patrolPoints;
    public float patrolSpeed = 1f;
    public float chaseSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int health = 100;
    public CapsuleCollider2D capsuleCollider;

    private int currentPatrolIndex;
    private Transform player;
    private float lastAttackTime;
    private bool facingRight = true;

    public bool IsPlayerPetrolArea { get => isPlayerPetrolArea; set => isPlayerPetrolArea = value; }
    public bool IsPlayerDetected { get => isPlayerDetected; set => isPlayerDetected = value; }
    public bool ChangingPatrolPoint { get => changingPatrolPoint; set => changingPatrolPoint = value; }
    public Transform[] PatrolPoints { get => patrolPoints; set => patrolPoints = value; }
    public float PatrolSpeed { get => patrolSpeed; set => patrolSpeed = value; }
    public float ChaseSpeed { get => chaseSpeed; set => chaseSpeed = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }
    public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
    public int Health { get => health; set => health = value; }
    public CapsuleCollider2D CapsuleCollider { get => capsuleCollider; set => capsuleCollider = value; }

    private void Start()
    {
        currentPatrolIndex = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerDetected = false;
        }
    }

    private void Update()
    {
        if (health <= 0)
        {
            Die();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isPlayerPetrolArea || isPlayerDetected)
        {
            ChasePlayer();

            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void ChasePlayer()
    {
        Vector2 targetPosition = player.position;
        FlipTowards(targetPosition);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
    }

    private async void Patrol()
    {
        Debug.Log("Patrolling");
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        FlipTowards(targetPoint.position);
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f && !isPlayerDetected &&
            !isPlayerPetrolArea && !changingPatrolPoint)
        {
            changingPatrolPoint = true;
            await UniTask.WaitForSeconds(1);
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            changingPatrolPoint = false;
        }
    }

    private void FlipTowards(Vector2 targetPosition)
    {
        if ((targetPosition.x > transform.position.x && !facingRight) || (targetPosition.x < transform.position.x && facingRight))
        {
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }

    public void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Attack logic here (e.g., reduce player health)
            Debug.Log("Attacking the player");
            lastAttackTime = Time.time;
        }
    }

    public void Die()
    {
        // Die logic here (e.g., play animation, destroy object)
        Debug.Log("Enemy died");
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
}