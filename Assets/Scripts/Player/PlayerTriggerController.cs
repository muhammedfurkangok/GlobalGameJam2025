using System;
using UnityEngine;

public class PlayerTriggerController : MonoBehaviour
{
    [Header("Tags")] 
    private const string meleeEnemyTag = "MeleeEnemy";
    private const string flyChargeEnemy = "FlyChargeEnemy";
    private const string rangeEnemyProjectile = "RangeProjectile";

    private void OnTriggerEnter2D(Collider2D other)
    {
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