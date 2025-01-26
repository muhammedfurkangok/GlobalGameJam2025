using UnityEngine;
using UnityEngine.SceneManagement;

public class ToyManager : MonoBehaviour
{
    [SerializeField]
    private Toy[] toys;

    private int collectedToys = 0;

    public SceneController SceneController;

    private void Start()
    {
        foreach (Toy toy in toys)
        {
            toy.OnCollected += ToyCollected;
        }
    }

    void ToyCollected()
    {
        collectedToys++;
        if (collectedToys == toys.Length)
        {
            Debug.Log("All toys collected!");
            foreach (Toy toy in toys)
            {
                toy.OnCollected -= ToyCollected;
            }
            SceneController.LoadNextScene();
        }

    }
    private void OnDestroy()
    {
        foreach (Toy toy in toys)
        {
            toy.OnCollected -= ToyCollected;
        }
    }
}