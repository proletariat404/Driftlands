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

			// 可选：内部排序
			foreach (var list in behaviorMap.Values)
				list.Sort((a, b) => a.sort_order.CompareTo(b.sort_order));

			Debug.Log($"成功加载 {wrapper.behaviors.Count} 条事件行为");
		}
		catch (Exception ex)
		{
			Debug.LogError($"加载事件行为数据出错: {ex.Message}");
		}
	}

	/// <summary>
	/// 获取某个事件的所有行为选项
	/// </summary>
	public List<BehaviorData> GetBehaviorsByEventId(int eventId)
	{
		return behaviorMap.TryGetValue(eventId, out var list) ? list : new List<BehaviorData>();
	}

	/// <summary>
	/// 获取所有行为（扁平列表）
	/// </summary>
	public List<BehaviorData> GetAllBehaviors()
	{
		List<BehaviorData> all = new();
		foreach (var list in behaviorMap.Values)
			all.AddRange(list);
		return all;
	}

	/// <summary>
	/// 根据条件筛选某个事件的行为（例：require:chiyou）
	/// </summary>
	public List<BehaviorData> GetValidBehaviors(int eventId, Func<string, bool> conditionChecker)
	{
		var list = GetBehaviorsByEventId(eventId);
		return list.FindAll(b => string.IsNullOrEmpty(b.condition) || conditionChecker(b.condition));
	}

	// ---------- 数据结构 ----------

	[Serializable]
	public class BehaviorData
	{
		public int event_id;
		public int behavior_id;
		public string display_text;
		public float rate;
		public int sort_order;
		public string condition;

		// 新增字段
		public string reward_id;       // 格式示例："[[1001,1]]"
		public string extra_reward_id; // 格式示例："[[2001,1]]"
		public string penalty_id;      // 文本惩罚描述，例如："攻击+30%，防御+30%"
	}

	[Serializable]
	private class BehaviorList
	{
		public List<BehaviorData> behaviors;
	}
}
