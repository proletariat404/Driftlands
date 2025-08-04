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

			// �ȼ����¼�������
			foreach (var e in eventList.events)
			{
				events[e.event_id] = e;
				e.interactions = new List<InteractionData>();
			}

			// ������Ϊ
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

			// ����
			foreach (var e in events.Values)
				e.interactions.Sort((a, b) => a.sort_order.CompareTo(b.sort_order));

			Debug.Log($"�ɹ����� {events.Count} �������¼�");
		}
		catch (Exception ex)
		{
			Debug.LogError($"���ؽ����¼����ݳ���: {ex.Message}");
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
			"ս��" => "fight",
			"˵��" => "persuade",
			"��թ" => "deceive",
			"����" => "intimidate",
			_ => label.ToLower()
		};
	}

	// ---------- �ڲ����ݽṹ ----------

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
        public string reward_id;        // ���
        public string extra_reward_id;  // ��ӣ������Ҫ��
        public string penalty_id;       // ���
    }

    [Serializable]
	private class BehaviorList { public List<BehaviorData> behaviors; }
}
