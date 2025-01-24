using System;
using UnityEngine;

public class PlayerDetectionForEnemy : MonoBehaviour
{
    public Enemy enemy;
    private readonly string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            enemy.isPlayerPetrolArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            enemy.isPlayerPetrolArea = false;
        }
    }
}