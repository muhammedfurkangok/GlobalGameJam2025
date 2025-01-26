using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    float startPos;
    public GameObject cam;
    public float parallaxEffect;
    
    void Start()
    {
        startPos = transform.position.x;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = (cam.transform.position.x * parallaxEffect);
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

    }
}
