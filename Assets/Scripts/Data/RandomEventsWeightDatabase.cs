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
			// json��һ�����飬��Ҫ�����ɶ�������JsonUtility�����л�
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

	// ��ȡ�����¼�Ȩ������
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
