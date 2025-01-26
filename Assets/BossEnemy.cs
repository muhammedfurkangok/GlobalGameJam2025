using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : MonoBehaviour, IEnemy
{
    public float chaseSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 3f;
    public int health = 500;
    public CapsuleCollider2D capsuleCollider;

    private Animator animator;
    private Transform player;
    private float lastAttackTime;
    private bool facingRight = true;

    public bool IsPlayerPetrolArea { get; set; }
    public bool IsPlayerDetected { get; set; }
    public bool ChangingPatrolPoint { get; set; }
    public bool IsPlatformEnemy { get; set; }
    public bool IsStunned { get; set; }
    public Transform[] PatrolPoints { get; set; }
    public float PatrolSpeed { get; set; }
    public Vector3 attackOffset;

    public CinemachineImpulseSource impulseSource;
    public SceneController sceneController;

    public Image bossHealthBarBackground;
    public Image bossHealthBarFill;

    public CinemachineCamera initCamera;
    public bool startSequence = true;

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

    public BoxCollider2D enemyCollider { get; set; }

    private void Start()
    {
        HealthBarInitializeAnimation();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void HealthBarInitializeAnimation()
    {
        bossHealthBarBackground.transform.DOScale(new Vector3(1, 1, 1), 0.5f).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                bossHealthBarFill.DOFillAmount(1, 4f).OnComplete(() =>
                {
                    startSequence = false;
                    StartGame();
                });
            });
    }

    private void HealhBarCloseAnimation()
    {
        bossHealthBarBackground.transform.DOScale(new Vector3(0, 1, 1), 0.5f).SetEase(Ease.InBack);
    }

    private void StartGame()
    {
        initCamera.gameObject.SetActive(false);
        Debug.Log("Game Started");
    }

    private void Update()
    {
        if (startSequence) return;

        if (IsStunned) return;

        ChaseAndAttackPlayer();
    }

    private void ChaseAndAttackPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        else
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetBool("IsRunning", false);
                Attack();
                lastAttackTime = Time.time;
                UniTask.Delay((int)(attackCooldown * 1000)).Forget();
            }
        }
    }

    public void ImpulseCinemachine()
    {
        impulseSource.GenerateImpulse();
    }

    private void ChasePlayer()
    {
        animator.SetBool("IsRunning", true);
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y); // Only chase on the x-axis
        FlipTowards(targetPosition);
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
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
        animator.SetTrigger("Attack");
    }

    private void CheckCollision()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position + attackOffset, attackRange,
            LayerMask.GetMask("Player"));

        foreach (var enemy in hitEnemies)
        {
            enemy.GetComponent<PlayerManager>().playerHealthController.TakeDamage(20);
        }
    }

    public void Stun()
    {
        if (IsStunned) return;

        IsStunned = true;
        animator.SetBool("IsRunning", false);
        animator.SetTrigger("Die");
        HealhBarCloseAnimation();
        sceneController.SceneEnd();
    }

    public void TakeDamage(int damage)
    {
        if (IsStunned) return;

        health -= damage;
        float newFillAmount = (float)health / 500;

        bossHealthBarFill.DOFillAmount(newFillAmount, 0.5f).SetEase(Ease.OutQuad);

        bossHealthBarBackground.transform.DOShakePosition(0.5f, new Vector3(10, 0, 0), 10, 90, false, true);

        if (health <= 0)
        {
            Stun();
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + attackOffset, attackRange);
    }
}