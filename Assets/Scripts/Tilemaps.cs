using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapScrollController : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap1;
    [SerializeField] private Tilemap tilemap2;
    [SerializeField] private Tilemap tilemap3;

    [SerializeField] private Camera targetCamera;

    private Transform[] grounds;
    private float tilemapWidth;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (tilemap1 == null || tilemap2 == null || tilemap3 == null)
        {
            Debug.LogError("Tilemap이 연결되지 않았습니다.");
            return;
        }

        // 타일맵 실제 크기 계산
        tilemap1.CompressBounds();

        tilemapWidth =
            tilemap1.cellBounds.size.x *
            tilemap1.layoutGrid.cellSize.x;

        Debug.Log("타일맵 길이 : " + tilemapWidth);

        grounds = new Transform[]
        {
            tilemap1.transform,
            tilemap2.transform,
            tilemap3.transform
        };

        // 자동 정렬
        tilemap1.transform.position = new Vector3(0, 0, 0);
        tilemap2.transform.position = new Vector3(tilemapWidth, 0, 0);
        tilemap3.transform.position = new Vector3(tilemapWidth * 2f, 0, 0);
    }

    void Update()
    {
        float cameraLeftX = GetCameraLeftX();

        foreach (Transform ground in grounds)
        {
            float rightEdgeX =
                ground.position.x + (tilemapWidth * 0.5f);

            // 카메라 왼쪽 밖으로 완전히 사라졌으면
            if (rightEdgeX < cameraLeftX)
            {
                Transform rightMost = GetRightMostGround();

                ground.position = new Vector3(
                    rightMost.position.x + tilemapWidth,
                    ground.position.y,
                    ground.position.z
                );
            }
        }
    }

    Transform GetRightMostGround()
    {
        Transform rightMost = grounds[0];

        foreach (Transform ground in grounds)
        {
            if (ground.position.x > rightMost.position.x)
            {
                rightMost = ground;
            }
        }

        return rightMost;
    }

    float GetCameraLeftX()
    {
        float distance =
            Mathf.Abs(
                targetCamera.transform.position.z -
                transform.position.z
            );

        return targetCamera
            .ViewportToWorldPoint(
                new Vector3(0f, 0.5f, distance)
            ).x;
    }
}