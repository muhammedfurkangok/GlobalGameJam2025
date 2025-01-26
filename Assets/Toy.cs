using UnityEngine;

public class Toy : MonoBehaviour
{
    public event System.Action OnCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Toy Trigger Enter");
        if (collision.CompareTag("Player"))
        {
            OnCollected?.Invoke();
            Destroy(this.gameObject);
        }
    }
}
