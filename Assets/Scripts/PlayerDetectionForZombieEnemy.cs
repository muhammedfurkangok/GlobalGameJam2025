using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDetectionForZombieEnemy : MonoBehaviour
{
    [FormerlySerializedAs("zombieEnemy")] public MeleeEnemy meleeEnemy;
    private readonly string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            meleeEnemy.isPlayerPetrolArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            meleeEnemy.isPlayerPetrolArea = false;
        }
    }
}