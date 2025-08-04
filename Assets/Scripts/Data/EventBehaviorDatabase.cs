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
        Debug.Log($"[EventBehaviorDatabase] ���Լ����ļ�: {path}");

        if (!File.Exists(path))
        {
            Debug.LogError($"EventBehaviorDatabase file not found: {path}");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            Debug.Log($"[EventBehaviorDatabase] JSON���ݳ���: {json.Length}");
            Debug.Log($"[EventBehaviorDatabase] JSON����ǰ200�ַ�: {json.Substring(0, Math.Min(200, json.Length))}");

            BehaviorList wrapper = JsonUtility.FromJson<BehaviorList>("{\"behaviors\":" + json + "}");
            Debug.Log($"[EventBehaviorDatabase] ������ {wrapper.behaviors?.Count} ����Ϊ");

            if (wrapper.behaviors == null)
            {
                Debug.LogError("[EventBehaviorDatabase] wrapper.behaviors Ϊ null");
                return;
            }

            foreach (var b in wrapper.behaviors)
            {
                Debug.Log($"[EventBehaviorDatabase] ������Ϊ: event_id={b.event_id}, display_text={b.display_text}, reward_id='{b.reward_id}', penalty_id='{b.penalty_id}'");

                if (!behaviorMap.ContainsKey(b.event_id))
                    behaviorMap[b.event_id] = new List<BehaviorData>();

                behaviorMap[b.event_id].Add(b);
            }

            // ��ÿ���ڲ��б�����
            foreach (var list in behaviorMap.Values)
                list.Sort((a, b) => a.sort_order.CompareTo(b.sort_order));

            Debug.Log($"�ɹ����� {wrapper.behaviors.Count} ���¼���Ϊ");

            // ��ӡ���ص����ݸ���
            foreach (var kvp in behaviorMap)
            {
                Debug.Log($"[EventBehaviorDatabase] �¼� {kvp.Key} �� {kvp.Value.Count} ����Ϊѡ��");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"�����¼���Ϊ���ݳ���: {ex.Message}");
            Debug.LogError($"��ջ����: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// ��ȡĳ���¼���������Ϊѡ��
    /// </summary>
    public List<BehaviorData> GetBehaviorsByEventId(int eventId)
    {
        var result = behaviorMap.TryGetValue(eventId, out var list) ? list : new List<BehaviorData>();
        Debug.Log($"[EventBehaviorDatabase] GetBehaviorsByEventId({eventId}) ���� {result.Count} ����Ϊ");
        return result;
    }

    /// <summary>
    /// ��ȡ������Ϊ�������ã�
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
        public string reward_id;       // ��ʽ�磺"[[1001,1]]"
        public string extra_reward_id; // ��ʽ�磺"[[2001,1]]"
        public string penalty_id;      // �ı��ͷ����������磺"����+30%������+30%"

        public override string ToString()
        {
            return $"BehaviorData(event_id:{event_id}, display_text:'{display_text}', reward_id:'{reward_id}', penalty_id:'{penalty_id}')";
        }
    }

    [Serializable]
    private class BehaviorList
    {
        public List<BehaviorData> behaviors;
    }
}