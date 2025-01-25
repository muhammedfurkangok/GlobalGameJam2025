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

            if (enemy.IsStunned && !enemy.IsPlatformEnemy)
            {
                other.transform.SetParent(this.transform);
                other.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBounce);
                transform.DOLocalMoveY(transform.localPosition.y + 10f, 5f).SetEase(Ease.OutBounce);
            }
            else if (enemy.IsStunned && enemy.IsPlatformEnemy)
            {
                playerDetected = true;
                other.transform.SetParent(this.transform);
                other.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBounce);
               transform.DOLocalMoveY(transform.localPosition.y + 1f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                enemy.TakeDamage(10);
            }
        }
    }
}