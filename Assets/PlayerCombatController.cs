using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform muzzle;
    public float projectileSpeed = 10f;

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        BubbleProjectile projectileScript = projectile.GetComponent<BubbleProjectile>();
        projectileScript.speed = projectileSpeed;
    }
}