using Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : SingletonMonoBehaviour<PlayerUIController>
{
    public PlayerHealthController playerHealthController;
    public Image healthBar;


    public void UpdateImageFill()
    {
        healthBar.fillAmount = playerHealthController.health / 100f;
    }
}