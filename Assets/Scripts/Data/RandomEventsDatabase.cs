using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class RandomEventsDatabase
{
    private Dictionary<int, RandomEventData> events = new();

    public RandomEventsDatabase(string fileName)
    {
        LoadEventsFromFile(fileName);
    }

    private void LoadEventsFromFile(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                RandomEventList list = JsonUtility.FromJson<RandomEventList>("{\"events\":" + json + "}");

                foreach (var e in list.events)
                {
                    events[e.event_id] = e;
                }

                Debug.Log($"成功加载 {events.Count} 个随机事件");
            }
            catch (Exception ex)
            {
                Debug.LogError($"加载随机事件数据时发生错误: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"RandomEventsDatabase file not found: {path}");
        }
    }

    /// <summary>
    /// 根据事件类型ID获取对应事件列表
    /// </summary>
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

    /// <summary>
    /// 根据事件ID获取事件数据
    /// </summary>
    public RandomEventData GetEventById(int eventId)
    {
        return events.TryGetValue(eventId, out var eventData) ? eventData : null;
    }

    /// <summary>
    /// 获取所有事件数据
    /// </summary>
    public IEnumerable<RandomEventData> GetAllEvents()
    {
        return events.Values;
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

        /// <summary>
        /// 获取完整的事件描述（包含奖励和惩罚信息）
        /// </summary>

    }

    [Serializable]
    private class RandomEventList
    {
        public List<RandomEventData> events;
    }
}