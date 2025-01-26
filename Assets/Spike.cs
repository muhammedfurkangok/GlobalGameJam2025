using UnityEngine;

public class Spike : MonoBehaviour
{
    [SerializeField] SceneController sceneController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        sceneController.SceneRestart();
    }
}
