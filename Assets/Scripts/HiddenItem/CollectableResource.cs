using UnityEngine;

public class CollectableResource : Interactable
{
    [Header("资源配置")]
    [Tooltip("资源类型，填0表示不使用类型")]
    public int resourceType = 1;

    [Tooltip("指定资源ID，填0表示不使用ID。如果同时填了类型和ID，优先使用ID")]
    public int resourceId = 0;

    private GameObject interactingPlayer;
    private int? selectedResourceId; // 缓存选中的资源ID

    protected override void OnInteract(GameObject player)
    {
        interactingPlayer = player;

        // 根据优先级获取资源ID：ID > 类型 > 错误
        selectedResourceId = GetResourceId();
        if (selectedResourceId == null)
        {
            Debug.LogWarning("没有找到对应的资源配置");
            return;
        }

        string resourceName = DropManager.Instance.GetResourceNameById(selectedResourceId.Value);
        int staminaCost = DropManager.Instance.GetStaminaCostByResourceId(selectedResourceId.Value);

        string message = $"当前矿脉为{resourceName}，是否消耗{staminaCost}点体力进行采集？";

        UIConfirmManager.Instance.ShowConfirm(message, OnConfirmCollect, OnCancelCollect);
    }

    /// <summary>
    /// 根据配置获取资源ID
    /// 优先级：resourceId > resourceType > null
    /// </summary>
    private int? GetResourceId()
    {
        // 优先使用指定的资源ID
        if (resourceId > 0)
        {
            // 验证资源ID是否存在
            if (DropManager.Instance.GetResourceNameById(resourceId) != "未知资源")
            {
                return resourceId;
            }
            else
            {
                Debug.LogWarning($"指定的资源ID {resourceId} 不存在，尝试使用资源类型");
            }
        }

        // 其次使用资源类型随机选择
        if (resourceType > 0)
        {
            return DropManager.Instance.GetRandomResourceIdByType(resourceType);
        }

        // 都没有配置
        Debug.LogError("资源类型和资源ID都未正确配置");
        return null;
    }

    private void OnConfirmCollect()
    {
        if (interactingPlayer == null || selectedResourceId == null) return;

        var playerStats = interactingPlayer.GetComponent<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerStats组件未找到");
            return;
        }

        int staminaCost = DropManager.Instance.GetStaminaCostByResourceId(selectedResourceId.Value);

        if (!playerStats.TryConsumeStamina(staminaCost))
        {
            UIHintManager.Instance?.ShowHint("体力不足，无法采集");
            return;
        }

        var drops = DropManager.Instance.GetDropByResourceId(selectedResourceId.Value);
        if (drops == null || drops.Count == 0)
        {
            Debug.LogWarning("没有找到掉落配置或没有掉落物品");
            return;
        }

        foreach (var drop in drops)
        {
            int baseAmount = drop.amount;
            int extraAmount = 0;
            int finalAmount = baseAmount;

            // 判断是否触发炎帝额外奖励
            if (playerStats.selectedTribe == TribeType.YanDi)
            {
                float chance = playerStats.extraGatherRewardChance;
                float rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < chance)
                {
                    // 触发额外奖励，倍率可根据需求调整，比如额外翻倍
                    float multiplier = 1.5f; // 这里示例为1.5倍
                    finalAmount = Mathf.CeilToInt(baseAmount * multiplier);
                    extraAmount = finalAmount - baseAmount;

                    Debug.Log($"炎帝天赋触发！物品{drop.itemData.item_name}额外奖励，基础数量{baseAmount}，最终数量{finalAmount}");

                    UIHintManager.Instance?.ShowHint(
                        $"炎帝恩赐！获得{drop.itemData.type_name}：{drop.itemData.item_name} x {finalAmount}（额外{extraAmount}个）");
                }
            }

            // 默认提示（无额外奖励时）
            if (finalAmount == baseAmount)
            {
                UIHintManager.Instance?.ShowHint(
                    $"获得{drop.itemData.type_name}：{drop.itemData.item_name} x {finalAmount}");
            }

            // 给玩家添加物品
            // InventoryManager.Instance?.AddItem(drop.itemData.item_id, finalAmount);
        }

        // 播放采集特效或动画...

        Destroy(gameObject);
    }

    private void OnCancelCollect()
    {
        UIHintManager.Instance?.ShowHint("取消采集");
    }

    #region Editor 辅助显示
#if UNITY_EDITOR
    private void OnValidate()
    {
        // 在编辑器中提供配置提示
        if (resourceId > 0 && resourceType > 0)
        {
            Debug.Log($"同时配置了资源类型({resourceType})和资源ID({resourceId})，将优先使用资源ID");
        }
        else if (resourceId <= 0 && resourceType <= 0)
        {
            Debug.LogWarning("资源类型和资源ID都未配置，请至少配置其中一个");
        }
    }
#endif
    #endregion
}