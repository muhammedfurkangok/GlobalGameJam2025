using System;
using UnityEngine;

public class PlayerTriggerController : MonoBehaviour
{
    [Header("Manager References")]
    [SerializeField] private PlayerHealthController playerHealthController;
    
    [Header("Tags")]
    private const string zombieDamageTag = "ZombieDamage";
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(zombieDamageTag))
        {
            playerHealthController.TakeDamage(10);
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
            playerHealthController.canTakeDamage = false;
        }
    }
}