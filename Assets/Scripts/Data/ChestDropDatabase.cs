using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChestDropDatabase
{
    private Dictionary<int, ChestDropData> chests = new();

    public ChestDropDatabase(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ChestList list = JsonUtility.FromJson<ChestList>("{\"chests\":" + json + "}");
            foreach (var chest in list.chests)
            {
                chest.ParseFields(); // 解析字符串数组和二维数组
                chests[chest.chest_id] = chest;
            }
        }
        else
        {
            Debug.LogError($"ChestDropDatabase file not found: {path}");
        }
    }

    public ChestDropData GetChestById(int id)
    {
        chests.TryGetValue(id, out var data);
        return data;
    }

    // 新增 - 根据类型筛选宝箱列表
    public List<ChestDropData> GetChestsByType(int chestType)
    {
        List<ChestDropData> results = new List<ChestDropData>();
        foreach (var chest in chests.Values)
        {
            if (chest.chest_type == chestType)
                results.Add(chest);
        }
        return results;
    }

    [System.Serializable]
    public class ChestDropData
    {
        public int chest_id;
        public int chest_type;

        public string item_types;
        public string weights;
        public string amount_range;

        [System.NonSerialized] public int[] parsedItemTypes;
        [System.NonSerialized] public int[] parsedWeights;
        [System.NonSerialized] public int[][] parsedAmountRanges;

        public void ParseFields()
        {
            parsedItemTypes = ParseIntArray(item_types);
            parsedWeights = ParseIntArray(weights);
            parsedAmountRanges = ParseRangeArray(amount_range);
        }

        private int[] ParseIntArray(string s)
        {
            s = s.Trim('[', ']');
            if (string.IsNullOrEmpty(s))
                return new int[0];
            string[] parts = s.Split(',');
            int[] result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                int.TryParse(parts[i].Trim(), out result[i]);
            return result;
        }

        private int[][] ParseRangeArray(string s)
        {
            s = s.Trim();
            if (string.IsNullOrEmpty(s) || s.Length < 5)
                return new int[0][];

            if (s.StartsWith("[") && s.EndsWith("]"))
                s = s.Substring(1, s.Length - 2);

            List<int[]> ranges = new List<int[]>();
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
    }

    [System.Serializable]
    private class ChestList
    {
        public List<ChestDropData> chests;
    }
}
