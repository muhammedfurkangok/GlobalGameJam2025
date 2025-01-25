using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDetectionForEnemy : MonoBehaviour
{
    [FormerlySerializedAs("enemy")] public ZombieEnemy zombieEnemy;
        private readonly string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            zombieEnemy.isPlayerPetrolArea = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            zombieEnemy.isPlayerPetrolArea = false;
        }
    }
}