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

	[System.Serializable]
	public class ChestDropData
	{
		public int chest_id;
		public int chest_type;

		// 改为 item_types，表示物品类型数组字符串，比如 "[1,2,3]"
		public string item_types;
		public string weights;       // "[2,2,2]"
		public string amount_range;  // "[[1,100],[1,100],[1,100]]"

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

		// 解析二维数组字符串，例如 "[[1,100],[1,100],[1,100]]"
		private int[][] ParseRangeArray(string s)
		{
			// 去除外层中括号
			s = s.Trim();
			if (string.IsNullOrEmpty(s) || s.Length < 5) // 最小格式 "[[a,b]]"
				return new int[0][];

			// 移除最外层中括号
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