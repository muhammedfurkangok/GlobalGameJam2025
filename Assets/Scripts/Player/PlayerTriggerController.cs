using System;
using UnityEngine;

public class PlayerTriggerController : MonoBehaviour
{
    
    [Header("Tags")]
    private const string zombieDamageTag = "Zombie";
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(zombieDamageTag))
        {
            PlayerManager.Instance.playerHealthController.TakeDamage(10);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(zombieDamageTag))
        {
           
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(zombieDamageTag))
        {
            PlayerManager.Instance.playerHealthController.canTakeDamage = true;
        }
    }
}