using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    public bool isPlayerPetrolArea;
    public bool isPlayerDetected;
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public int health = 100;
    public CapsuleCollider2D capsuleCollider;

    private int currentPatrolIndex;
    private Transform player;
    private float lastAttackTime;

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
        transform.position = Vector2.MoveTowards(transform.position, player.position, patrolSpeed * Time.deltaTime);
    }


    private void Patrol()
    {
        Debug.Log("Patrolling");
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        transform.position =
            Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            // Attack logic here (e.g., reduce player health)
            Debug.Log("Attacking the player");
            lastAttackTime = Time.time;
        }
    }

    private void Die()
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