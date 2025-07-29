using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceDropDatabase
{
    private Dictionary<int, ResourceData> resources = new();

    public ResourceDropDatabase(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ResourceList list = JsonUtility.FromJson<ResourceList>("{\"resources\":" + json + "}");
            foreach (var res in list.resources)
            {
                res.ParseFields();
                resources[res.resource_id] = res;
            }
        }
        else
        {
            Debug.LogError($"ResourceDropDatabase file not found: {path}");
        }
    }

    public ResourceData GetResourceById(int id)
    {
        resources.TryGetValue(id, out var data);
        return data;
    }

    // ✅ 新增：通过 resource_type 获取所有资源
    public List<ResourceData> GetResourcesByType(int resourceType)
    {
        List<ResourceData> results = new();
        foreach (var res in resources.Values)
        {
            if (res.resource_type == resourceType)
                results.Add(res);
        }
        return results;
    }

    [Serializable]
    public class ResourceData
    {
        public int resource_id;
        public int resource_type;
        public string item_types;     // "[[2,2001],[2]]"
        public string weights;        // "[1,2]"
        public string amount_range;   // "[[1,100],[1,50]]"
        public string resource_type_name;
        public int stamina_cost; // 新增字段，对应配置表中的体力消耗


        [NonSerialized] public int[][] parsedItemTypes;
        [NonSerialized] public int[] parsedWeights;
        [NonSerialized] public int[][] parsedAmountRanges;

        public void ParseFields()
        {
            parsedItemTypes = ParseItemTypeArray(item_types);
            parsedWeights = ParseIntArray(weights);
            parsedAmountRanges = ParseRangeArray(amount_range);
        }

        private int[] ParseIntArray(string s)
        {
            s = s.Trim('[', ']');
            if (string.IsNullOrEmpty(s)) return Array.Empty<int>();
            string[] parts = s.Split(',');
            int[] result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                int.TryParse(parts[i].Trim(), out result[i]);
            return result;
        }

        private int[][] ParseRangeArray(string s)
        {
            s = s.Trim();
            if (string.IsNullOrEmpty(s) || s.Length < 5) return new int[0][];
            if (s.StartsWith("[") && s.EndsWith("]"))
                s = s.Substring(1, s.Length - 2);

            List<int[]> ranges = new();
            int start = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '[')
                    start = i;
                else if (s[i] == ']')
                {
                    string sub = s.Substring(start + 1, i - start - 1);
                    var parts = sub.Split(',');
                    if (parts.Length == 2 &&
                        int.TryParse(parts[0].Trim(), out int low) &&
                        int.TryParse(parts[1].Trim(), out int high))
                    {
                        ranges.Add(new int[] { low, high });
                    }
                }
            }
            return ranges.ToArray();
        }

        private int[][] ParseItemTypeArray(string s)
        {
            List<int[]> result = new();
            s = s.Trim();
            if (string.IsNullOrEmpty(s)) return result.ToArray();

            if (s.StartsWith("[") && s.EndsWith("]"))
                s = s.Substring(1, s.Length - 2);

            int start = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '[') start = i;
                else if (s[i] == ']')
                {
                    string sub = s.Substring(start + 1, i - start - 1);
                    var parts = sub.Split(',');
                    List<int> pair = new();
                    foreach (var part in parts)
                    {
                        if (int.TryParse(part.Trim(), out int val))
                            pair.Add(val);
                    }
                    if (pair.Count > 0)
                        result.Add(pair.ToArray());
                }
            }
            return result.ToArray();
        }
    }

    [Serializable]
    private class ResourceList
    {
        public List<ResourceData> resources;
    }
}
