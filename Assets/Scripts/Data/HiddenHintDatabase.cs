using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HiddenHintDatabase
{
	private Dictionary<int, HiddenHintData> hints = new();

	public HiddenHintDatabase(string fileName)
	{
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			string wrapped = "{\"hints\":" + json + "}"; // 包装成数组
			HiddenHintWrapper wrapper = JsonUtility.FromJson<HiddenHintWrapper>(wrapped);
			foreach (var h in wrapper.hints)
				hints[h.ID] = h;
		}
		else
		{
			Debug.LogError($"[HiddenHintDatabase] 找不到提示文件: {path}");
		}
	}

	public HiddenHintData GetHintById(int id)
	{
		hints.TryGetValue(id, out var data);
		return data;
	}

	[System.Serializable]
	public class HiddenHintData
	{
		public int ID;
		public string HintText;
		public float Duration;
		public int Type;
		public string Column4;
	}

	[System.Serializable]
	private class HiddenHintWrapper
	{
		public List<HiddenHintData> hints;
	}
}