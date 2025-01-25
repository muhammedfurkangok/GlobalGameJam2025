using UnityEngine;

public class BubbleProjectile : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;


    public void SetDirection(Vector2 newDirection)
    {
        direction = newDirection.normalized;
    }

    private void Update()
    {
        transform.Translate(direction * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit");
            other.GetComponent<IEnemy>().TakeDamage(25);
            Destroy(gameObject);
        }
    }
}