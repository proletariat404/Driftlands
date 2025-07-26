using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0, 0, -10f);
    public Tilemap mapTilemap;

    private Vector2 minLimit;
    private Vector2 maxLimit;

    private float camHeight;
    private float camWidth;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        if (mapTilemap != null)
        {
            BoundsInt cellBounds = mapTilemap.cellBounds;

            // cellBounds.max 是包含边界的下一格，所以减一格才是最后一个有效tile
            Vector3Int minCell = cellBounds.min;
            Vector3Int maxCell = new Vector3Int(cellBounds.max.x - 1, cellBounds.max.y - 1, cellBounds.max.z);

            Vector3 minWorld = mapTilemap.CellToWorld(minCell);
            Vector3 maxWorld = mapTilemap.CellToWorld(maxCell) + mapTilemap.cellSize; // 加上一个tile大小，取右上角点

            minLimit = new Vector2(minWorld.x, minWorld.y);
            maxLimit = new Vector2(maxWorld.x, maxWorld.y);
        }
        else
        {
            Debug.LogWarning("CameraFollow：未设置 Tilemap！");
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        float clampedX = Mathf.Clamp(desiredPosition.x, minLimit.x + camWidth, maxLimit.x - camWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minLimit.y + camHeight, maxLimit.y - camHeight);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (mapTilemap == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(minLimit.x, minLimit.y), new Vector3(maxLimit.x, minLimit.y));
        Gizmos.DrawLine(new Vector3(minLimit.x, minLimit.y), new Vector3(minLimit.x, maxLimit.y));
        Gizmos.DrawLine(new Vector3(maxLimit.x, minLimit.y), new Vector3(maxLimit.x, maxLimit.y));
        Gizmos.DrawLine(new Vector3(minLimit.x, maxLimit.y), new Vector3(maxLimit.x, maxLimit.y));
    }
}
