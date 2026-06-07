using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public float scrollSpeed = 5.0f;

    [SerializeField] private int health = 50;
    public int Health => health;

    private int score = 0;
    public int Score => score;

    private float distance = 0f;
    public float Distance => distance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI distanceText;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private GameObject clearText;

    [SerializeField] private GameObject[] spawnPrefabs;

    [SerializeField] private float itemSpawnInterval = 2f;
    [SerializeField] private Vector2 itemSpawnYRange = new Vector2(-2f, 2f);

    [SerializeField] private Transform itemParent;
    [SerializeField] private Camera targetCamera;

    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject gameOverText;

    private float itemSpawnTimer;
    private bool isGameCleared = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        if (clearText != null)
        {
            clearText.SetActive(false);
        }

        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }

        if (scoreText != null)
        {
            scoreText.text = "0";
        }

        if (distanceText != null)
        {
            distanceText.text = "0m";
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = health;
        }

        UpdateHPUI();
    }

    public void AddScore(int amount = 1)
    {
        score += amount;

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void TakeDamage(int damage = 1)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
        }

        Debug.Log("남은 체력: " + health);

        UpdateHPUI();

        if (health <= 0)
        {
            if (gameOverText != null)
            {
                gameOverText.SetActive(true);
            }

            if (restartButton != null)
            {
                restartButton.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

    public void Heal(int amount)
    {
        health += amount;

        if (health > 50)
        {
            health = 50;
        }

        UpdateHPUI();
    }

    void UpdateHPUI()
    {
        if (hpSlider != null)
        {
            hpSlider.value = health;
        }

        if (hpText != null)
        {
            hpText.text = health + " / " + hpSlider.maxValue;
        }
    }

    void Update()
    {
        // 거리 계산 및 클리어 조건
        if (!isGameCleared)
        {
            if (Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.RightArrow))
            {
                distance += scrollSpeed * Time.deltaTime;
            }

            if (distanceText != null)
            {
                distanceText.text =
                    Mathf.FloorToInt(distance).ToString() + "m";
            }

            if (distance >= 320f)
            {
                isGameCleared = true;

                if (clearText != null)
                {
                    clearText.SetActive(true);
                }

                if (restartButton != null)
                {
                    restartButton.SetActive(true);
                }

                Time.timeScale = 0f;
            }
        }

        // 아이템 생성
        if (spawnPrefabs == null || spawnPrefabs.Length == 0)
        {
            return;
        }

        itemSpawnTimer += Time.deltaTime;

        if (itemSpawnTimer < itemSpawnInterval)
        {
            return;
        }

        itemSpawnTimer = 0f;

        SpawnItem();
    }

    void SpawnItem()
    {
        int randomIndex = Random.Range(0, spawnPrefabs.Length);
        GameObject selectedPrefab = spawnPrefabs[randomIndex];

        if (selectedPrefab == null)
        {
            return;
        }

        float spawnX = GetCameraRightX();
        float spawnY = 0f;

        ItemMover moverComponent =
            selectedPrefab.GetComponent<ItemMover>();

        if (moverComponent != null &&
            moverComponent.isObstacle)
        {
            spawnY = -3.8f;
        }
        else
        {
            spawnY = Random.Range(
                itemSpawnYRange.x,
                itemSpawnYRange.y
            );
        }

        Vector3 spawnPosition =
            new Vector3(spawnX, spawnY, 0f);

        GameObject item = Instantiate(
            selectedPrefab,
            spawnPosition,
            Quaternion.identity,
            itemParent
        );

        ItemMover mover = item.GetComponent<ItemMover>();

        if (mover == null)
        {
            mover = item.AddComponent<ItemMover>();
        }

        mover.scrollSpeed = scrollSpeed;

        mover.targetCamera =
            targetCamera != null
            ? targetCamera
            : Camera.main;
    }

    float GetCameraRightX()
    {
        Camera cam =
            targetCamera != null
            ? targetCamera
            : Camera.main;

        if (cam == null)
        {
            return 10f;
        }

        float camDistance =
            Mathf.Abs(cam.transform.position.z);

        return cam.ViewportToWorldPoint(
            new Vector3(1f, 0.5f, camDistance)
        ).x;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}