using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerTriggerController : MonoBehaviour
{
    [Header("Tags")] private const string meleeEnemyTag = "MeleeEnemy";
    private const string flyChargeEnemy = "FlyChargeEnemy";
    private const string rangeEnemyProjectile = "RangeProjectile";
    private const string platform = "Platform";

    public float jumpForce = 500f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(platform))
        {
            var forceDirection = Vector2.up;
            PlayerManager.Instance.playerRigidbody2D.AddForce(forceDirection * jumpForce, ForceMode2D.Force);

            StartCoroutine(AdjustGravityScale(PlayerManager.Instance.playerRigidbody2D, 2f, 3f));

            var platformObjectParent = other.transform.parent;
            platformObjectParent.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                Destroy(platformObjectParent.gameObject);
            });
        }


        if (other.CompareTag(rangeEnemyProjectile))
        {
            PlayerManager.Instance.playerHealthController.TakeDamage(10);
            Destroy(other.gameObject);
        }


        // if (other.CompareTag(meleeEnemyTag))
        // {
        //     PlayerManager.Instance.playerHealthController.TakeDamage(10);
        // }
        //
        // if (other.CompareTag(flyChargeEnemy))
        // {
        //     PlayerManager.Instance.playerHealthController.TakeDamage(10);
        // }
    }

    private IEnumerator AdjustGravityScale(Rigidbody2D rb, float newGravityScale, float duration)
    {
        float originalGravityScale = rb.gravityScale;
        rb.gravityScale = newGravityScale;
        yield return new WaitForSeconds(duration);
        rb.gravityScale = originalGravityScale;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
    }


    private void OnTriggerExit2D(Collider2D other)
    {
    }
}