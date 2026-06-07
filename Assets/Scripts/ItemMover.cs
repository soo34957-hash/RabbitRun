using UnityEngine;

public class ItemMover : MonoBehaviour
{
    [HideInInspector]
    public float scrollSpeed = 5f;

    [HideInInspector]
    public Camera targetCamera;

    // 인스펙터 창에서 장애물인지 코인인지 직접 체크할 수 있게 public
    public bool isObstacle = false;
    public bool isHeart = false;
    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.isTrigger = true;
    }

    void Update()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x < GetCameraLeftX())
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (isObstacle)
        {
            GameManager.Instance?.TakeDamage();
        }
        else if (isHeart)
        {
            GameManager.Instance?.Heal(1);
        }
        else
        {
            GameManager.Instance?.AddScore();
        }

        Destroy(gameObject);
    }

    float GetCameraLeftX()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null)
            return float.NegativeInfinity;

        float distance = Mathf.Abs(cam.transform.position.z - transform.position.z);
        return cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, distance)).x;
    }
}