using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ItemDatabase
{
	private Dictionary<int, ItemData> items = new();

	public ItemDatabase(string fileName)
	{
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			ItemList list = JsonUtility.FromJson<ItemList>("{\"items\":" + json + "}");
			foreach (var item in list.items)
				items[item.item_id] = item;
		}
		else
		{
			Debug.LogError($"ItemDatabase file not found: {path}");
		}
	}

	public ItemData GetItemById(int id)
	{
		items.TryGetValue(id, out var item);
		return item;
	}

	public List<ItemData> GetItemsByType(int type)
	{
		List<ItemData> results = new();
		foreach (var item in items.Values)
		{
			if (item.item_type == type)
				results.Add(item);
		}
		return results;
	}

	[System.Serializable]
	public class ItemData
	{
		public int item_id;
		public string item_name;
		public int item_type;
		public string type_name;
		public int weight;
	}

	[System.Serializable]
	private class ItemList
	{
		public List<ItemData> items;
	}
}
