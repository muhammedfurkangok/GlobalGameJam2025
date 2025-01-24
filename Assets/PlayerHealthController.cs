using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public int health = 100;
    public bool canTakeDamage = true;

    public void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}