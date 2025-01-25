using Cysharp.Threading.Tasks;
using UnityEngine;

public class RangeEnemy : MonoBehaviour, IEnemy
{
    public bool isPlayerPetrolArea;
    public bool isPlayerDetected;
    public bool changingPatrolPoint;
    public Transform[] patrolPoints;
    public float patrolSpeed = 1f;
    public float attackRange = 5f; // Adjusted attack range for ranged enemy
    public float attackCooldown = 1f;
    public int health = 100;
    public CapsuleCollider2D capsuleCollider;
    public GameObject projectilePrefab; // Projectile prefab reference
    public Transform firePoint; // Projectile spawn point

    private int currentPatrolIndex;
    private Transform player;
    private float lastAttackTime;
    private bool facingRight = true;

    public bool IsPlayerPetrolArea
    {
        get => isPlayerPetrolArea;
        set => isPlayerPetrolArea = value;
    }

    public bool IsPlayerDetected
    {
        get => isPlayerDetected;
        set => isPlayerDetected = value;
    }

    public bool ChangingPatrolPoint
    {
        get => changingPatrolPoint;
        set => changingPatrolPoint = value;
    }

    public Transform[] PatrolPoints
    {
        get => patrolPoints;
        set => patrolPoints = value;
    }

    public float PatrolSpeed
    {
        get => patrolSpeed;
        set => patrolSpeed = value;
    }

    public float ChaseSpeed { get; set; }

    public float AttackRange
    {
        get => attackRange;
        set => attackRange = value;
    }

    public float AttackCooldown
    {
        get => attackCooldown;
        set => attackCooldown = value;
    }

    public int Health
    {
        get => health;
        set => health = value;
    }

    public CapsuleCollider2D CapsuleCollider
    {
        get => capsuleCollider;
        set => capsuleCollider = value;
    }

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

        if (isPlayerDetected)
        {
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else
            {
                // Optionally, move closer to the player but stay within the attack range
                MoveTowardsPlayer();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector2 targetPosition = player.position;
        FlipTowards(targetPosition);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);
    }

    private async void Patrol()
    {
        Debug.Log("Patrolling");
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        FlipTowards(targetPoint.position);
        transform.position =
            Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

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
        if ((targetPosition.x > transform.position.x && !facingRight) ||
            (targetPosition.x < transform.position.x && facingRight))
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
            // Launch the projectile
            LaunchProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void LaunchProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            // Instantiate the projectile and move it towards the player
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Vector2 direction = (player.position - firePoint.position).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 10f; // Adjust projectile speed as needed

            Debug.Log("Projectile launched towards the player");
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