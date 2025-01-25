using DG.Tweening;
using UnityEngine;

public class BubbleCapture : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;
    private bool playerDetected;


    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    private void Update()
    {
        if (playerDetected) return;
        
        transform.Translate(direction * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<IEnemy>();

            if (enemy.IsStunned)
            {
                other.transform.SetParent(this.transform);
                
                transform.DOLocalMoveY(transform.localPosition.y + 10f, 5f).SetEase(Ease.OutBounce);
            }
            else
            {
                enemy.TakeDamage(10);
            }
        }
    }
}