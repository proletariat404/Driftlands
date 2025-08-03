using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EventBehaviorDatabase
{
	private Dictionary<int, List<BehaviorData>> behaviorMap = new();

	public EventBehaviorDatabase(string fileName)
	{
		LoadFromFile(fileName);
	}

	private void LoadFromFile(string fileName)
	{
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		if (!File.Exists(path))
		{
			Debug.LogError($"EventBehaviorDatabase file not found: {path}");
			return;
		}

		try
		{
			string json = File.ReadAllText(path);
			BehaviorList wrapper = JsonUtility.FromJson<BehaviorList>("{\"behaviors\":" + json + "}");

			foreach (var b in wrapper.behaviors)
			{
				if (!behaviorMap.ContainsKey(b.event_id))
					behaviorMap[b.event_id] = new List<BehaviorData>();

				behaviorMap[b.event_id].Add(b);
			}

			// ��ѡ���ڲ�����
			foreach (var list in behaviorMap.Values)
				list.Sort((a, b) => a.sort_order.CompareTo(b.sort_order));

			Debug.Log($"�ɹ����� {wrapper.behaviors.Count} ���¼���Ϊ");
		}
		catch (Exception ex)
		{
			Debug.LogError($"�����¼���Ϊ���ݳ���: {ex.Message}");
		}
	}

	/// <summary>
	/// ��ȡĳ���¼���������Ϊѡ��
	/// </summary>
	public List<BehaviorData> GetBehaviorsByEventId(int eventId)
	{
		return behaviorMap.TryGetValue(eventId, out var list) ? list : new List<BehaviorData>();
	}

	/// <summary>
	/// ��ȡ������Ϊ����ƽ�б�
	/// </summary>
	public List<BehaviorData> GetAllBehaviors()
	{
		List<BehaviorData> all = new();
		foreach (var list in behaviorMap.Values)
			all.AddRange(list);
		return all;
	}

	/// <summary>
	/// ��������ɸѡĳ���¼�����Ϊ������require:chiyou��
	/// </summary>
	public List<BehaviorData> GetValidBehaviors(int eventId, Func<string, bool> conditionChecker)
	{
		var list = GetBehaviorsByEventId(eventId);
		return list.FindAll(b => string.IsNullOrEmpty(b.condition) || conditionChecker(b.condition));
	}

	// ---------- ���ݽṹ ----------

	[Serializable]
	public class BehaviorData
	{
		public int event_id;
		public int behavior_id;
		public string display_text;
		public float rate;
		public int sort_order;
		public string condition;

		// �����ֶ�
		public string reward_id;       // ��ʽʾ����"[[1001,1]]"
		public string extra_reward_id; // ��ʽʾ����"[[2001,1]]"
		public string penalty_id;      // �ı��ͷ����������磺"����+30%������+30%"
	}

	[Serializable]
	private class BehaviorList
	{
		public List<BehaviorData> behaviors;
	}
}
