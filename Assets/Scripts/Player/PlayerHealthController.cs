using Cysharp.Threading.Tasks;
using DG.Tweening;
using Runtime.Extensions;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public int health = 100;
    public bool canTakeDamage = true;
    public GameObject bubbleRescue;

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
        bubbleRescue.SetActive(true);
        bubbleRescue.transform.DOScale(2, 1f).OnComplete(() =>
        {
            transform.DOLocalMoveY(10, 10f).OnComplete(() =>
            {
               
            });
            DOVirtual.DelayedCall( 1.5f, () =>
            {
                PlayerManager.Instance.sceneController.SceneRestart();
            });
        });
    }
}