using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomEventSystem
{
	private RandomEventsWeightDatabase weightDb;
	private RandomEventsDatabase eventsDb;
	private ItemDatabase itemDb; // �����Ʒ���ݿ�����

	/// <summary>�����������ʣ�0~1��</summary>
	private float baseTriggerChance = 0.3f;

	/// <summary>����ֵ��Խ��Խ�ã���λ�㣨1������ֵ=1Ȩ�ص�λ��</summary>
	private int luckValue = 0;

	/// <summary>
	/// �¼�������Ļص�ί�У������ǿ��ܴ������¼����ݣ�null��ʾδ�����¼�
	/// </summary>
	public event Action<RandomEventsDatabase.RandomEventData, string> OnEventTriggered; // �޸ģ���������ı�����

	public RandomEventSystem(RandomEventsWeightDatabase weightDatabase, RandomEventsDatabase eventsDatabase, ItemDatabase itemDatabase)
	{
		weightDb = weightDatabase;
		eventsDb = eventsDatabase;
		itemDb = itemDatabase; // ��ʼ����Ʒ���ݿ�
	}

	/// <summary>
	/// ���û����������ʣ�0~1��
	/// </summary>
	public void SetBaseTriggerChance(float chance)
	{
		baseTriggerChance = Mathf.Clamp01(chance);
	}

	/// <summary>
	/// ��������ֵ����0����Խ��
	/// </summary>
	public void SetLuckValue(int luck)
	{
		luckValue = Mathf.Max(0, luck);
	}

	/// <summary>
	/// ���Դ�������¼������ش������¼����ݣ����δ��������null���������ص�
	/// </summary>
	public RandomEventsDatabase.RandomEventData TryTriggerEvent()
	{
		// �����жϣ��������ʶ���������ֵ��Ȩ��Ӱ�죩
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

		// �����������¼��ı�����������/�ͷ���Ϣ��
		string fullEventText = GenerateFullEventText(triggeredEvent);

		OnEventTriggered?.Invoke(triggeredEvent, fullEventText);
		return triggeredEvent;
	}

	/// <summary>
	/// ���ɰ�������/�ͷ���Ϣ�������¼��ı�
	/// </summary>
	private string GenerateFullEventText(RandomEventsDatabase.RandomEventData eventData)
	{
		string fullText = eventData.event_text;

		// ������
		if (eventData.reward_item_id > 0 && eventData.reward_amount > 0)
		{
			var rewardItem = itemDb?.GetItemById(eventData.reward_item_id);
			if (rewardItem != null)
			{
				fullText += $"\n\n<color=green>����������� {rewardItem.item_name} x{eventData.reward_amount}</color>";
			}
			else
			{
				fullText += $"\n\n<color=green>�������������ƷID:{eventData.reward_item_id} x{eventData.reward_amount}</color>";
			}
		}

		// ����ͷ�
		if (!string.IsNullOrEmpty(eventData.penalty_id) && eventData.penalty_id != "0")
		{
			fullText += $"\n\n<color=red>���ͷ���{GetPenaltyDescription(eventData.penalty_id)}</color>";
		}

		return fullText;
	}

	/// <summary>
	/// ���ݳͷ�ID��ȡ�ͷ�����
	/// </summary>
	private string GetPenaltyDescription(string penaltyId)
	{
		// ������Ը�����ĳͷ�ϵͳ�����ʵ��
		// �����ǿ۳���Ǯ���������Ե�
		switch (penaltyId.ToLower())
		{
			case "money_loss":
				return "��ʧ��Ǯ";
			case "health_loss":
				return "��ʧ����ֵ";
			case "energy_loss":
				return "��ʧ����";
			case "item_loss":
				return "��ʧ��Ʒ";
			default:
				return $"�ͷ�Ч��: {penaltyId}";
		}
	}

	/// <summary>
	/// ����Ȩ�غ�����ֵ��Ȩ�����ѡ��һ���¼�����
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
	/// ��������ֵ��Ȩ���޸��¼����͵�Ȩ�أ�����ֵÿ1�� = 1Ȩ�ص�λ��
	/// ������Ч��Ȩ��+����ֵ�����治��Ȩ��-����ֵ���������䣬Ȩ�����Ϊ0
	/// </summary>
	private int AdjustWeightByLuck(RandomEventsWeightDatabase.RandomEventWeightData evt, int luckValue)
	{
		if (evt.event_type_text == "��������")
		{
			return evt.event_weight + luckValue;
		}
		else if (evt.event_type_text == "���治��")
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