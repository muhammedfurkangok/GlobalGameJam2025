using Cysharp.Threading.Tasks;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour, IEnemy
{
    public bool isPlayerPetrolArea;
    public bool isPlayerDetected;
    public bool changingPatrolPoint;
    public Transform[] patrolPoints;
    public float patrolSpeed = 1f;
    public float chaseSpeed = 2f;
    public float attackRange = 0.75f;
    public float attackCooldown = 1f;
    public int health = 100;
    public CapsuleCollider2D capsuleCollider;

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

    [SerializeField] private bool isPlatformEnemy;
    public bool IsPlatformEnemy
    {
        get => isPlatformEnemy;
        set => isPlatformEnemy = value;
    }

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

    public float ChaseSpeed
    {
        get => chaseSpeed;
        set => chaseSpeed = value;
    }

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
        if (health <= 0)
        {
            Stun();
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
        animator.SetBool("IsRunning", true);
        Vector2 targetPosition = player.position;
        FlipTowards(targetPosition);

        float distanceToPlayer = Vector2.Distance(transform.position, targetPosition);
        if (distanceToPlayer > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
        }
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
            animator.SetTrigger("Attack");

            Debug.Log("Attacking the player");
            lastAttackTime = Time.time;
        }
    }

    public void Stun()
    {
        IsStunned = true;

        Debug.Log("Enemy died");
    }

    public void TakeDamage(int damage)
    {
        animator.SetTrigger("Hit");
        health -= damage;
        if (health <= 0)
        {
            Stun();
        }
    }
}