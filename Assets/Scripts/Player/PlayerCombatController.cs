using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject ChargingBubblePrefab;
    public Transform muzzle;
    public float projectileSpeed;
    public float captureBubbleSpeed;
    public float chargeSpeedMultiplier;

    private bool isCharging = false;
    private float chargeStartTime;
    public GameObject ChargingBubble { get; private set; }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetMouseButtonDown(1))
        {
            StartCharging();
        }

        if (Input.GetMouseButtonUp(1) && isCharging)
        {
            ShootCharged();
        }

        if (isCharging)
        {
            float chargeTime = Time.time - chargeStartTime;
            float scale = Mathf.Clamp01(chargeTime);
            ChargingBubble.transform.localScale = new Vector3(scale, scale, scale);
            ChargingBubble.transform.position = muzzle.position;

            // Set charging animation
            PlayerManager.Instance.animator.SetBool("IsChargingBubble", true);
        }
    }

    private async void Shoot()
    {
        PlayerManager.Instance.animator.SetTrigger("Shoot");
        await UniTask.WaitForSeconds(0.3f);
        SoundManager.Instance.PlayOneShotSound(SoundType.CharacterAttack);

        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        BubbleProjectile projectileScript = projectile.GetComponent<BubbleProjectile>();
        projectileScript.SetDirection(PlayerManager.Instance.playerController.GetFacingDirection());
        projectileScript.speed = projectileSpeed;
    }

    private void StartCharging()
    {
        isCharging = true;
        chargeStartTime = Time.time;
        ChargingBubble = Instantiate(ChargingBubblePrefab, muzzle.position, muzzle.rotation);
        ChargingBubble.transform.SetParent(muzzle.transform);
        ChargingBubble.transform.localScale = Vector3.zero;

        PlayerManager.Instance.animator.SetBool("IsChargingBubble", true);
    }

    private void ShootCharged()
    {
        if (ChargingBubble.transform.localScale.x < 0.5f)
        {
            ChargingBubble.transform.DOScale(0, 0.5f).OnComplete(() => { Destroy(ChargingBubble); });
            ChargingBubble = null;
            isCharging = false;

            PlayerManager.Instance.animator.SetBool("IsChargingBubble", false);
            SoundManager.Instance.PlayOneShotSound(SoundType.CharacterAttack);
            return;
        }

        ChargingBubble.transform.SetParent(null);
        isCharging = false;
        float chargeTime = Time.time - chargeStartTime;
        BubbleCapture bubbleCapture = ChargingBubble.GetComponent<BubbleCapture>();
        bubbleCapture.SetDirection(PlayerManager.Instance.playerController.GetFacingDirection());
        bubbleCapture.speed = captureBubbleSpeed + (chargeSpeedMultiplier * chargeTime);
        ChargingBubble = null;

        PlayerManager.Instance.animator.SetBool("IsChargingBubble", false);
    }
}