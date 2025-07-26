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

        // ���ӻ���ӡ���
        Debug.Log($"{gameObject.name} bounds: {tilemap.localBounds}");
    }

    void Start()
    {
        FixBounds();
    }
}
