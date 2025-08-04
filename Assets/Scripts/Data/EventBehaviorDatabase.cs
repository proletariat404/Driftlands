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
        Debug.Log($"[EventBehaviorDatabase] 尝试加载文件: {path}");

        if (!File.Exists(path))
        {
            Debug.LogError($"EventBehaviorDatabase file not found: {path}");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            Debug.Log($"[EventBehaviorDatabase] JSON内容长度: {json.Length}");
            Debug.Log($"[EventBehaviorDatabase] JSON内容前200字符: {json.Substring(0, Math.Min(200, json.Length))}");

            BehaviorList wrapper = JsonUtility.FromJson<BehaviorList>("{\"behaviors\":" + json + "}");
            Debug.Log($"[EventBehaviorDatabase] 解析到 {wrapper.behaviors?.Count} 个行为");

            if (wrapper.behaviors == null)
            {
                Debug.LogError("[EventBehaviorDatabase] wrapper.behaviors 为 null");
                return;
            }

            foreach (var b in wrapper.behaviors)
            {
                Debug.Log($"[EventBehaviorDatabase] 处理行为: event_id={b.event_id}, display_text={b.display_text}, reward_id='{b.reward_id}', penalty_id='{b.penalty_id}'");

                if (!behaviorMap.ContainsKey(b.event_id))
                    behaviorMap[b.event_id] = new List<BehaviorData>();

                behaviorMap[b.event_id].Add(b);
            }

            // 对每个内部列表排序
            foreach (var list in behaviorMap.Values)
                list.Sort((a, b) => a.sort_order.CompareTo(b.sort_order));

            Debug.Log($"成功加载 {wrapper.behaviors.Count} 条事件行为");

            // 打印加载的数据概览
            foreach (var kvp in behaviorMap)
            {
                Debug.Log($"[EventBehaviorDatabase] 事件 {kvp.Key} 有 {kvp.Value.Count} 个行为选项");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载事件行为数据出错: {ex.Message}");
            Debug.LogError($"堆栈跟踪: {ex.StackTrace}");
        }
    }

    /// <summary>
    /// 获取某个事件的所有行为选项
    /// </summary>
    public List<BehaviorData> GetBehaviorsByEventId(int eventId)
    {
        var result = behaviorMap.TryGetValue(eventId, out var list) ? list : new List<BehaviorData>();
        Debug.Log($"[EventBehaviorDatabase] GetBehaviorsByEventId({eventId}) 返回 {result.Count} 个行为");
        return result;
    }

    /// <summary>
    /// 获取所有行为（调试用）
    /// </summary>
    public List<BehaviorData> GetAllBehaviors()
    {
        List<BehaviorData> all = new();
        foreach (var list in behaviorMap.Values)
            all.AddRange(list);
        return all;
    }

    /// <summary>
    /// 根据条件筛选某个事件的行为（比如require:chiyou）
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
        public string reward_id;       // 格式如："[[1001,1]]"
        public string extra_reward_id; // 格式如："[[2001,1]]"
        public string penalty_id;      // 文本惩罚描述，比如："攻击+30%，防御+30%"

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