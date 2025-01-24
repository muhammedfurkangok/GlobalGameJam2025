using Cysharp.Threading.Tasks;
using Runtime.Extensions;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public int health = 100;
    public bool canTakeDamage = true;

    public async void TakeDamage(int damage)
    {
        if (!canTakeDamage) return;

        canTakeDamage = false;
        health -= damage;
        PlayerUIController.Instance.UpdateImageFill();
        //damage anim
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