// TreasureChest.cs
using UnityEngine;

public class TreasureChest : Interactable
{
    protected override void OnInteract(GameObject player)
    {
        Debug.Log("宝箱自动打开并给予奖励");
        // 这里写动画、奖励逻辑等
        Destroy(gameObject);
    }
}
