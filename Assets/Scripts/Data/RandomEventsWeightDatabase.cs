using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomEventsWeightDatabase
{
	private Dictionary<int, RandomEventWeightData> events = new();

	public RandomEventsWeightDatabase(string fileName)
	{
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			// json是一个数组，需要包裹成对象再用JsonUtility反序列化
			RandomEventWeightList list = JsonUtility.FromJson<RandomEventWeightList>("{\"events\":" + json + "}");
			foreach (var e in list.events)
			{
				events[e.event_id] = e;
			}
		}
		else
		{
			Debug.LogError($"RandomEventsWeightDatabase file not found: {path}");
		}
	}

	// 获取所有事件权重数据
	public IEnumerable<RandomEventWeightData> GetAllEvents()
	{
		return events.Values;
	}

	[Serializable]
	public class RandomEventWeightData
	{
		public int event_id;
		public string event_type_text;
		public int event_weight;
	}

	[Serializable]
	private class RandomEventWeightList
	{
		public List<RandomEventWeightData> events;
	}
}
