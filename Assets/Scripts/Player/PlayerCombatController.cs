using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform muzzle;
    public float projectileSpeed = 10f;
    public float chargeSpeedMultiplier = 2f;
    private bool isCharging = false;
    private float chargeStartTime;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

        if (Input.GetButtonDown("Fire2"))
        {
            StartCharging();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            ShootCharged();
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
    }

    private void ShootCharged()
    {
        isCharging = false;
        float chargeTime = Time.time - chargeStartTime;
        GameObject bubbleForCarry = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        BubbleCapture bubbleCapture = bubbleForCarry.GetComponent<BubbleCapture>();
        bubbleCapture.SetDirection(PlayerManager.Instance.playerController.GetFacingDirection());
        bubbleCapture.speed = projectileSpeed + (chargeSpeedMultiplier * chargeTime);
    }
}