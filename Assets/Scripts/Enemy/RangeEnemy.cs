using Cysharp.Threading.Tasks;
using UnityEngine;

public class RangeEnemy : MonoBehaviour, IEnemy
{
    public bool isPlayerPetrolArea;
    public bool isPlayerDetected;
    public bool changingPatrolPoint;
    public Transform[] patrolPoints;
    public float patrolSpeed = 1f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    public int health = 100;
    public CapsuleCollider2D capsuleCollider;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Animator animator;
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

    public bool IsPlatformEnemy { get; set; }

    public bool IsStunned { get; set; }

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

    public BoxCollider2D enemyCollider { get; set; }

    private void Start()
    {
        animator = GetComponent<Animator>();
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
        if (IsStunned) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isPlayerDetected)
        {
            if (distanceToPlayer <= attackRange)
            {
                Attack();
            }
            else
            {
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
        animator.SetBool("IsRunning", true);
        Vector2 targetPosition = player.position;
        FlipTowards(targetPosition);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, patrolSpeed * Time.deltaTime);
    }

    private async void Patrol()
    {
        animator.SetBool("IsRunning", true);
        Transform targetPoint = patrolPoints[currentPatrolIndex];
        FlipTowards(targetPoint.position);
        transform.position =
            Vector2.MoveTowards(transform.position, targetPoint.position, patrolSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f && !isPlayerDetected &&
            !isPlayerPetrolArea && !changingPatrolPoint)
        {
            changingPatrolPoint = true;
            animator.SetBool("IsRunning", false);
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
            animator.SetBool("IsRunning", false);
            SoundManager.Instance.PlayOneShotSound(SoundType.RangeCharacterAttack, 0.05f);
            animator.SetTrigger("Attack");
            LaunchProjectile();
            lastAttackTime = Time.time;
        }
    }

    private void LaunchProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Vector2 direction = (player.position - firePoint.position).normalized;
            projectile.GetComponent<Rigidbody2D>().linearVelocity = direction * 10f;

            Debug.Log("Projectile launched towards the player");
        }
    }

    public void Stun()
    {
        IsStunned = true;

        if (transform.parent != null)
        {
            var oldParent = transform.parent.gameObject;
            transform.SetParent(null);
            Destroy(oldParent);
        }

        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Die");
    }

    public void TakeDamage(int damage)
    {
        if (IsStunned) return;

        SoundManager.Instance.PlayOneShotSound(SoundType.RangeCharacterHit, 0.05f);
        animator.SetTrigger("Hit");
        health -= damage;
        if (health <= 0)
        {
            Stun();
        }
    }
}