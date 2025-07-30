using UnityEngine;
using System;

public class RandomEventManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RandomEventsWeightDatabase weightDb;
    [SerializeField] private RandomEventsDatabase eventsDb;
    [SerializeField] private ItemDatabase itemDb;

    [Header("UI Reference")]
    [SerializeField] private RandomEventUI eventUI;

    private RandomEventSystem eventSystem;

    public event Action<RandomEventsDatabase.RandomEventData> OnEventTriggered;

    public void SetDatabases(
        RandomEventsWeightDatabase weightDb,
        RandomEventsDatabase eventsDb,
        ItemDatabase itemDb)
    {
        this.weightDb = weightDb;
        this.eventsDb = eventsDb;
        this.itemDb = itemDb;
    }

    public void SetEventUI(RandomEventUI ui)
    {
        this.eventUI = ui;
    }

    private void Start()
    {
        InitializeEventSystem();
    }

    private void InitializeEventSystem()
    {
        if (weightDb == null || eventsDb == null)
        {
            Debug.LogError("Event databases not set!");
            return;
        }

        eventSystem = new RandomEventSystem(weightDb, eventsDb);
        eventSystem.OnEventTriggered += HandleSystemEvent;
    }

    /// <summary>
    /// 设置基础触发概率（0~1）
    /// </summary>
    public void SetBaseTriggerChance(float chance)
    {
        if (eventSystem != null) eventSystem.SetBaseTriggerChance(chance);
    }

    /// <summary>
    /// 设置幸运值（≥0），影响正负面事件权重
    /// </summary>
    public void SetLuckValue(int luck)
    {
        if (eventSystem != null) eventSystem.SetLuckValue(luck);
    }

    /// <summary>
    /// 触发随机事件
    /// </summary>
    public void TriggerRandomEvent()
    {
        if (eventSystem != null)
        {
            eventSystem.TryTriggerEvent();
        }
    }

    private void HandleSystemEvent(RandomEventsDatabase.RandomEventData eventData)
    {
        // 处理UI显示
        if (eventData != null && eventUI != null)
        {
            string description = GenerateEventDescription(eventData);
            eventUI.ShowEvent(description);
        }

        // 转发事件给订阅者
        OnEventTriggered?.Invoke(eventData);
    }

    /// <summary>
    /// 生成包含物品名称的完整事件描述
    /// </summary>
    public string GenerateEventDescription(RandomEventsDatabase.RandomEventData eventData)
    {
        if (eventData == null) return "未触发任何事件";

        string description = eventData.event_text;
        description += "\n" + new string('-', 20);

        // 奖励处理
        if (eventData.reward_item_id > 0 && eventData.reward_amount > 0)
        {
            var rewardItem = itemDb?.GetItemById(eventData.reward_item_id);
            string itemName = rewardItem != null ?
                rewardItem.item_name :
                $"未知物品(ID:{eventData.reward_item_id})";

            description += $"\n<color=green>【奖励】获得 {itemName} × {eventData.reward_amount}</color>";
        }

        // 惩罚处理
        if (!string.IsNullOrEmpty(eventData.penalty_id) && eventData.penalty_id != "0")
        {
            description += $"\n<color=red>【惩罚】{GetPenaltyDescription(eventData.penalty_id)}</color>";
        }

        // 无效果的情况
        if ((eventData.reward_item_id <= 0 || eventData.reward_amount <= 0) &&
            (string.IsNullOrEmpty(eventData.penalty_id) || eventData.penalty_id == "0"))
        {
            description += "\n<color=gray>【结果】无特殊效果</color>";
        }

        return description;
    }

    private string GetPenaltyDescription(string penaltyId)
    {
        return penaltyId.ToLower() switch
        {
            "money_loss" => "损失金钱",
            "health_loss" => "损失生命值",
            "energy_loss" or "stamina_loss" => "损失体力",
            "item_loss" => "丢失物品",
            "time_loss" => "浪费时间",
            "reputation_loss" => "声望下降",
            _ => $"惩罚效果: {penaltyId}"
        };
    }

    private void OnDestroy()
    {
        if (eventSystem != null)
        {
            eventSystem.OnEventTriggered -= HandleSystemEvent;
        }
    }
}