using UnityEngine;

public class TreasureChest : Interactable
{
    public int chestType = 0;  // 宝箱类型，填0表示无效
    public int chestId = 0;    // 宝箱ID，填0表示无效

    protected override void OnInteract(GameObject player)
    {
        Debug.Log("宝箱打开，开始掉落物品");

        // 使用新的接口：优先ID，其次类型
        var drops = DropManager.Instance.GetDropByChestTypeOrId(chestType, chestId);
        if (drops == null || drops.Count == 0)
        {
            Debug.LogWarning("没有找到掉落配置");
            return;
        }

        foreach (var drop in drops)
        {
            // 这里可以根据具体需求，也可以直接用于背包添加逻辑
            Debug.Log($"获得{drop.itemData.type_name}：{drop.itemData.item_name} × {drop.amount}");

            // 弹出 UI 提示显示
            UIHintManager.Instance?.ShowHint($"获得{drop.itemData.type_name}：{drop.itemData.item_name} × {drop.amount}");
        }

        // 宝箱开启动画或特效可以放这里

        Destroy(gameObject); // 开完销毁
    }
}