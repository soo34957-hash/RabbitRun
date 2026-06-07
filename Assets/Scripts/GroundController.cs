using UnityEngine;

public class GroundScroller : MonoBehaviour
{
    public float speed = 8f;
    public float chunkWidth = 41f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < -41f)
        {
            transform.position += Vector3.right * chunkWidth * 3f;
        }
    }
}