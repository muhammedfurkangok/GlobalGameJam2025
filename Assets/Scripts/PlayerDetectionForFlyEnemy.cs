using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDetectionForFlyEnemy : MonoBehaviour
{
    public FlyEnemy flyEnemy;
    private readonly string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            flyEnemy.isPlayerPetrolArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            flyEnemy.isPlayerPetrolArea = false;
        }
    }
}