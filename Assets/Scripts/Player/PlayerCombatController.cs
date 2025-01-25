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
    public GameObject ChargingBubble { get; private set; } // Public property with private setter

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
            float scale = Mathf.Clamp01(chargeTime); // scale'ı 0'dan 1'e kadar ayarla
            ChargingBubble.transform.localScale = new Vector3(scale, scale, scale);
            ChargingBubble.transform.position = muzzle.position; // Update position to follow the muzzle
        }
    }

    private void Shoot()
    {
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
        ChargingBubble.transform.localScale = Vector3.zero; // Başlangıçta scale 0
    }

    private void ShootCharged()
    {
        ChargingBubble.transform.SetParent(null); // Parent'tan çıkar
        isCharging = false;
        float chargeTime = Time.time - chargeStartTime;
        BubbleCapture bubbleCapture = ChargingBubble.GetComponent<BubbleCapture>();
        bubbleCapture.SetDirection(PlayerManager.Instance.playerController.GetFacingDirection());
        bubbleCapture.speed = captureBubbleSpeed + (chargeSpeedMultiplier * chargeTime);
        ChargingBubble = null; // Referansı sıfırla
    }
}