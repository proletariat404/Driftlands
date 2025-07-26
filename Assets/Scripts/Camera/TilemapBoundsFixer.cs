using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapBoundsFixer : MonoBehaviour
{
    [ContextMenu("Fix Tilemap Bounds")]
    void FixBounds()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();

        // 可视化打印结果
        Debug.Log($"{gameObject.name} bounds: {tilemap.localBounds}");
    }

    void Start()
    {
        FixBounds();
    }
}
