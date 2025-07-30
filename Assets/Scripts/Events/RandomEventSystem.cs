using UnityEngine;
using System.Collections.Generic;

public class RandomEventSystem
{
    private RandomEventsWeightDatabase weightDb;
    private RandomEventsDatabase eventsDb;

    /// <summary>基础触发概率（0~1）</summary>
    private float baseTriggerChance = 0.3f;

    /// <summary>幸运值，影响正面和负面事件的权重</summary>
    private int luckValue = 0;

    public event System.Action<RandomEventsDatabase.RandomEventData> OnEventTriggered;

    public RandomEventSystem(RandomEventsWeightDatabase weightDatabase,
                            RandomEventsDatabase eventsDatabase)
    {
        weightDb = weightDatabase;
        eventsDb = eventsDatabase;
    }

    /// <summary>
    /// 设置基础触发概率（0~1）
    /// </summary>
    public void SetBaseTriggerChance(float chance)
    {
        baseTriggerChance = Mathf.Clamp01(chance);
    }

    /// <summary>
    /// 设置幸运值（≥0），影响正负面事件权重
    /// </summary>
    public void SetLuckValue(int luck)
    {
        luckValue = Mathf.Max(0, luck);
    }

    /// <summary>
    /// 尝试触发随机事件，返回触发的事件数据，如果未触发返回null
    /// </summary>
    public RandomEventsDatabase.RandomEventData TryTriggerEvent()
    {
        // 概率判断（触发概率不受幸运值的直接影响）
        if (UnityEngine.Random.value > baseTriggerChance)
        {
            OnEventTriggered?.Invoke(null);
            return null;
        }

        var eventType = GetRandomEventTypeByWeight(luckValue);
        if (eventType == null)
        {
            OnEventTriggered?.Invoke(null);
            return null;
        }

        var eventList = eventsDb.GetEventsByTypeId(eventType.event_id);
        if (eventList == null || eventList.Count == 0)
        {
            OnEventTriggered?.Invoke(null);
            return null;
        }

        int idx = UnityEngine.Random.Range(0, eventList.Count);
        var triggeredEvent = eventList[idx];

        OnEventTriggered?.Invoke(triggeredEvent);
        return triggeredEvent;
    }

    /// <summary>
    /// 根据权重和幸运值的权衡随机选择一个事件类型
    /// </summary>
    private RandomEventsWeightDatabase.RandomEventWeightData GetRandomEventTypeByWeight(int luckValue)
    {
        var allEvents = new List<RandomEventsWeightDatabase.RandomEventWeightData>(weightDb.GetAllEvents());

        List<(RandomEventsWeightDatabase.RandomEventWeightData evt, int adjustedWeight)> weightedList = new();

        foreach (var e in allEvents)
        {
            int adjusted = AdjustWeightByLuck(e, luckValue);
            if (adjusted > 0)
                weightedList.Add((e, adjusted));
        }

        int totalWeight = 0;
        foreach (var pair in weightedList)
            totalWeight += pair.adjustedWeight;

        if (totalWeight <= 0)
            return null;

        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        int sum = 0;
        foreach (var pair in weightedList)
        {
            sum += pair.adjustedWeight;
            if (randomWeight < sum)
                return pair.evt;
        }
        return null;
    }

    /// <summary>
    /// 根据幸运值调整事件类型的权重（幸运值每1点 = 1权重单位）
    /// 积极正面效果权重+幸运值，消极不利权重-幸运值，其他不变，权重最低为0
    /// </summary>
    private int AdjustWeightByLuck(RandomEventsWeightDatabase.RandomEventWeightData evt, int luckValue)
    {
        if (evt.event_type_text == "积极正面")
        {
            return evt.event_weight + luckValue;
        }
        else if (evt.event_type_text == "消极不利")
        {
            int adjusted = evt.event_weight - luckValue;
            return Mathf.Max(adjusted, 0);
        }
        else
        {
            return evt.event_weight;
        }
    }
}