using UnityEngine;

public class BubbleProjectile : MonoBehaviour
{
    public float speed = 10f;

    private void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Handle collision logic here
        Destroy(gameObject);
    }
}