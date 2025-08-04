using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameEventsDatabase
{
	private Dictionary<int, GameEventData> events = new();

	public GameEventsDatabase(string eventsFileName, string behaviorsFileName)
	{
		LoadFromFile(eventsFileName, behaviorsFileName);
	}

	private void LoadFromFile(string eventsFileName, string behaviorsFileName)
	{
		string eventsPath = Path.Combine(Application.streamingAssetsPath, eventsFileName);
		string behaviorsPath = Path.Combine(Application.streamingAssetsPath, behaviorsFileName);

		if (!File.Exists(eventsPath) || !File.Exists(behaviorsPath))
		{
			Debug.LogError($"GameEventsDatabase file not found:\n{eventsPath}\n{behaviorsPath}");
			return;
		}

		try
		{
			string eventsJson = File.ReadAllText(eventsPath);
			string behaviorsJson = File.ReadAllText(behaviorsPath);

			var eventList = JsonUtility.FromJson<GameEventList>("{\"events\":" + eventsJson + "}");
			var behaviorList = JsonUtility.FromJson<BehaviorList>("{\"behaviors\":" + behaviorsJson + "}");

			// 先加载事件主数据
			foreach (var e in eventList.events)
			{
				events[e.event_id] = e;
				e.interactions = new List<InteractionData>();
			}

			// 加入行为
			foreach (var b in behaviorList.behaviors)
			{
				if (!events.ContainsKey(b.event_id)) continue;

				var i = new InteractionData
				{
					key = GuessKeyFromLabel(b.display_text),
					label = b.display_text,
					rate = b.rate,
					condition = b.condition,
					sort_order = b.sort_order
				};

				events[b.event_id].interactions.Add(i);
			}

			// 排序
			foreach (var e in events.Values)
				e.interactions.Sort((a, b) => a.sort_order.CompareTo(b.sort_order));

			Debug.Log($"成功加载 {events.Count} 个交互事件");
		}
		catch (Exception ex)
		{
			Debug.LogError($"加载交互事件数据出错: {ex.Message}");
		}
	}

	public GameEventData GetEventById(int eventId)
	{
		return events.TryGetValue(eventId, out var data) ? data : null;
	}

	public IEnumerable<GameEventData> GetAllEvents() => events.Values;

	private string GuessKeyFromLabel(string label)
	{
		return label switch
		{
			"战斗" => "fight",
			"说服" => "persuade",
			"欺诈" => "deceive",
			"恐吓" => "intimidate",
			_ => label.ToLower()
		};
	}

	// ---------- 内部数据结构 ----------

	[Serializable]
	public class GameEventData
	{
		public int event_id;
		public int event_type;
		public int target_type;
		public string title;
		public string description;
		public List<InteractionData> interactions;
	}

	[Serializable]
	public class InteractionData
	{
		public string key;
		public string label;
		public float rate;
		public string condition;
		public int sort_order;
	}

	[Serializable]
	private class GameEventList { public List<GameEventData> events; }

	[Serializable]
    private class BehaviorData
    {
        public int event_id;
        public int behavior_id;
        public string display_text;
        public float rate;
        public int sort_order;
        public string condition;
        public string reward_id;        // 添加
        public string extra_reward_id;  // 添加（如果需要）
        public string penalty_id;       // 添加
    }

    [Serializable]
	private class BehaviorList { public List<BehaviorData> behaviors; }
}
