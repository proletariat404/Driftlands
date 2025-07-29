using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventSystem
{
	private RandomEventsWeightDatabase weightDb;
	private RandomEventsDatabase eventsDb;
	private ItemDatabase itemDb; // 添加物品数据库引用

	/// <summary>基础触发概率（0~1）</summary>
	private float baseTriggerChance = 0.3f;

	/// <summary>幸运值，越高越好，单位点（1点幸运值=1权重单位）</summary>
	private int luckValue = 0;

	/// <summary>
	/// 事件触发后的回调委托，参数是可能触发的事件数据，null表示未触发事件
	/// </summary>
	public event Action<RandomEventsDatabase.RandomEventData, string> OnEventTriggered; // 修改：添加完整文本参数

	public RandomEventSystem(RandomEventsWeightDatabase weightDatabase, RandomEventsDatabase eventsDatabase, ItemDatabase itemDatabase)
	{
		weightDb = weightDatabase;
		eventsDb = eventsDatabase;
		itemDb = itemDatabase; // 初始化物品数据库
	}

	/// <summary>
	/// 设置基础触发概率（0~1）
	/// </summary>
	public void SetBaseTriggerChance(float chance)
	{
		baseTriggerChance = Mathf.Clamp01(chance);
	}

	/// <summary>
	/// 设置幸运值（≥0），越高
	/// </summary>
	public void SetLuckValue(int luck)
	{
		luckValue = Mathf.Max(0, luck);
	}

	/// <summary>
	/// 尝试触发随机事件，返回触发的事件数据，如果未触发返回null，并触发回调
	/// </summary>
	public RandomEventsDatabase.RandomEventData TryTriggerEvent()
	{
		// 概率判断（触发概率独立于幸运值的权重影响）
		if (UnityEngine.Random.value > baseTriggerChance)
		{
			OnEventTriggered?.Invoke(null, "");
			return null;
		}

		var eventType = GetRandomEventTypeByWeight(luckValue);
		if (eventType == null)
		{
			OnEventTriggered?.Invoke(null, "");
			return null;
		}

		var eventList = eventsDb.GetEventsByTypeId(eventType.event_id);
		if (eventList == null || eventList.Count == 0)
		{
			OnEventTriggered?.Invoke(null, "");
			return null;
		}

		int idx = UnityEngine.Random.Range(0, eventList.Count);
		var triggeredEvent = eventList[idx];

		// 生成完整的事件文本（包含奖励/惩罚信息）
		string fullEventText = GenerateFullEventText(triggeredEvent);

		OnEventTriggered?.Invoke(triggeredEvent, fullEventText);
		return triggeredEvent;
	}

	/// <summary>
	/// 生成包含奖励/惩罚信息的完整事件文本
	/// </summary>
	private string GenerateFullEventText(RandomEventsDatabase.RandomEventData eventData)
	{
		string fullText = eventData.event_text;

		// 处理奖励
		if (eventData.reward_item_id > 0 && eventData.reward_amount > 0)
		{
			var rewardItem = itemDb?.GetItemById(eventData.reward_item_id);
			if (rewardItem != null)
			{
				fullText += $"\n\n<color=green>【奖励】获得 {rewardItem.item_name} x{eventData.reward_amount}</color>";
			}
			else
			{
				fullText += $"\n\n<color=green>【奖励】获得物品ID:{eventData.reward_item_id} x{eventData.reward_amount}</color>";
			}
		}

		// 处理惩罚
		if (!string.IsNullOrEmpty(eventData.penalty_id) && eventData.penalty_id != "0")
		{
			fullText += $"\n\n<color=red>【惩罚】{GetPenaltyDescription(eventData.penalty_id)}</color>";
		}

		return fullText;
	}

	/// <summary>
	/// 根据惩罚ID获取惩罚描述
	/// </summary>
	private string GetPenaltyDescription(string penaltyId)
	{
		// 这里可以根据你的惩罚系统设计来实现
		// 可能是扣除金钱、降低属性等
		switch (penaltyId.ToLower())
		{
			case "money_loss":
				return "损失金钱";
			case "health_loss":
				return "损失生命值";
			case "energy_loss":
				return "损失体力";
			case "item_loss":
				return "丢失物品";
			default:
				return $"惩罚效果: {penaltyId}";
		}
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
	/// 根据幸运值的权衡修改事件类型的权重（幸运值每1点 = 1权重单位）
	/// 即正面效果权重+幸运值，负面不利权重-幸运值，其他不变，权重最低为0
	/// </summary>
	private int AdjustWeightByLuck(RandomEventsWeightDatabase.RandomEventWeightData evt, int luckValue)
	{
		if (evt.event_type_text == "积极正向")
		{
			return evt.event_weight + luckValue;
		}
		else if (evt.event_type_text == "负面不利")
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