using UnityEngine;
using System.Collections.Generic;

public class RandomEventSystem
{
	private RandomEventsWeightDatabase weightDb;
	private RandomEventsDatabase eventsDb;

	private float baseTriggerChance = 0.3f;
	private int luckValue = 0;

	public event System.Action<RandomEventsDatabase.RandomEventData> OnEventTriggered;

	public RandomEventSystem(RandomEventsWeightDatabase weightDatabase,
							RandomEventsDatabase eventsDatabase)
	{
		weightDb = weightDatabase;
		eventsDb = eventsDatabase;
	}

	public void SetBaseTriggerChance(float chance)
	{
		baseTriggerChance = Mathf.Clamp01(chance);
	}

	public void SetLuckValue(int luck)
	{
		luckValue = Mathf.Max(0, luck);
	}

	public RandomEventsDatabase.RandomEventData TryTriggerEvent()
	{
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

	private int AdjustWeightByLuck(RandomEventsWeightDatabase.RandomEventWeightData evt, int luckValue)
	{
		if (evt.event_type_text == "积极正面")
			return evt.event_weight + luckValue;
		else if (evt.event_type_text == "消极不利")
			return Mathf.Max(evt.event_weight - luckValue, 0);
		else
			return evt.event_weight;
	}
}
