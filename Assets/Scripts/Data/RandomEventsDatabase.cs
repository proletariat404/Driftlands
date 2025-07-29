using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomEventsDatabase
{
	private Dictionary<int, RandomEventData> events = new();

	public RandomEventsDatabase(string fileName)
	{
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			// 同样包裹成对象方便解析
			RandomEventList list = JsonUtility.FromJson<RandomEventList>("{\"events\":" + json + "}");
			foreach (var e in list.events)
			{
				events[e.event_id] = e;
			}
		}
		else
		{
			Debug.LogError($"RandomEventsDatabase file not found: {path}");
		}
	}

	// 根据事件类型ID获取对应事件列表
	public List<RandomEventData> GetEventsByTypeId(int typeId)
	{
		List<RandomEventData> result = new();
		foreach (var e in events.Values)
		{
			if (e.event_type_id == typeId)
				result.Add(e);
		}
		return result;
	}

	[Serializable]
	public class RandomEventData
	{
		public int event_id;
		public int event_type_id;
		public string event_type_text;
		public string event_text;
		public int reward_item_id;
		public float reward_amount;
		public string penalty_id;
		public string Column7;
	}

	[Serializable]
	private class RandomEventList
	{
		public List<RandomEventData> events;
	}
}
