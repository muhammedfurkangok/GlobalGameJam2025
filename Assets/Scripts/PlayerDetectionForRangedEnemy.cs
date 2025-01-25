using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDetectionForRangedEnemy : MonoBehaviour
{
    public RangeEnemy rangeEnemy;
    private readonly string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            rangeEnemy.isPlayerPetrolArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            rangeEnemy.isPlayerPetrolArea = false;
        }
    }
}