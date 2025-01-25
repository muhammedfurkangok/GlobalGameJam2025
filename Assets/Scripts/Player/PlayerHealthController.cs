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
        PlayerManager.Instance.animator.SetTrigger("Hit");
        health -= damage;
        PlayerManager.Instance.playerUIController.UpdateImageFill();
        if (health <= 0)
        {
            Die();
        }

        await UniTask.WaitForSeconds(1f);
        canTakeDamage = true;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}