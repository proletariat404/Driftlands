using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

public class DropManager : MonoBehaviour
{
	public static DropManager Instance { get; private set; }

	public string itemTableFileName = "item.json";
	public string chestTableFileName = "chest.json";

	private Dictionary<int, ItemData> itemDict = new();
	private Dictionary<int, ChestDropData> chestDict = new();

	private void Awake()
	{
		if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;

		LoadAllData();
	}

	void LoadAllData()
	{
		// 路径拼接
		string itemPath = Path.Combine(Application.streamingAssetsPath, itemTableFileName);
		string chestPath = Path.Combine(Application.streamingAssetsPath, chestTableFileName);

		// 读取文件
		string itemJson = File.ReadAllText(itemPath);
		string chestJson = File.ReadAllText(chestPath);

		// 解析
		var itemList = JsonUtility.FromJson<ItemListWrapper>("{\"items\":" + itemJson + "}").items;
		foreach (var item in itemList)
			itemDict[item.item_id] = item;

		var chestList = JsonUtility.FromJson<ChestListWrapper>("{\"chests\":" + chestJson + "}").chests;
		foreach (var chest in chestList)
			chestDict[chest.chest_id] = chest;
	}

	public List<DropResult> GetDropByChestId(int chestId)
	{
		if (!chestDict.TryGetValue(chestId, out var chest)) return null;

		var itemIds = ParseIntArray(chest.item_ids);
		var weights = ParseIntArray(chest.weights);
		var amounts = ParseIntArray(chest.amount_range);

		int selectedIndex = GetRandomIndexByWeight(weights);
		int itemId = itemIds[selectedIndex];
		int amount = amounts[selectedIndex];

		var item = itemDict.TryGetValue(itemId, out var itemData) ? itemData : null;

		return new List<DropResult> {
			new DropResult { itemData = item, amount = amount }
		};
	}

	int[] ParseIntArray(string raw) =>
		raw.Trim('[', ']').Split(',').Select(s => int.Parse(s)).ToArray();

	int GetRandomIndexByWeight(int[] weights)
	{
		int total = weights.Sum();
		int rand = UnityEngine.Random.Range(0, total);
		int acc = 0;
		for (int i = 0; i < weights.Length; i++)
		{
			acc += weights[i];
			if (rand < acc) return i;
		}
		return weights.Length - 1;
	}

	[Serializable] class ItemListWrapper { public List<ItemData> items; }
	[Serializable] class ChestListWrapper { public List<ChestDropData> chests; }

	[Serializable]
	public class ItemData
	{
		public int item_id;
		public string item_name;
		public int item_type;
	}

	[Serializable]
	public class ChestDropData
	{
		public int chest_id;
		public int chest_type;
		public string item_ids;      // "[1,2,3]"
		public string weights;       // "[2,2,2]"
		public string amount_range;  // "[100,100,100]"
	}

	public class DropResult
	{
		public ItemData itemData;
		public int amount;
	}
}
