using DG.Tweening;
using Runtime.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public Image healthBar;
    public RectTransform healthBarRectTransform;

    public void UpdateImageFill()
    {
        healthBar.DOFillAmount(PlayerManager.Instance.playerHealthController.health / 100f, 0.5f);
        ShakeHealthBar();
    }

    private void ShakeHealthBar()
    {
        healthBarRectTransform.DOShakePosition(0.5f, new Vector3(10, 10, 0), 10, 90, false, true);
    }
}