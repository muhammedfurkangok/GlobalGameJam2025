using System;
using DG.Tweening;
using UnityEngine;

public class PlayerTriggerController : MonoBehaviour
{
    [Header("Tags")] private const string meleeEnemyTag = "MeleeEnemy";
    private const string flyChargeEnemy = "FlyChargeEnemy";
    private const string rangeEnemyProjectile = "RangeProjectile";
    private const string platform = "Platform";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(platform))
        {
            Debug.Log("Platform hit");
            var forceDirection = Vector2.up; // Force direction is upwards
            PlayerManager.Instance.playerRigidbody2D.AddForce(forceDirection * 10, ForceMode2D.Impulse);
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


    private void OnTriggerStay2D(Collider2D other)
    {
    }


    private void OnTriggerExit2D(Collider2D other)
    {
    }
}