using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropManager : MonoBehaviour
{
	public static DropManager Instance { get; private set; }

	public string itemTableFileName = "item.json";
	public string chestTableFileName = "chest.json";

	private ItemDatabase itemDatabase;
	private ChestDropDatabase chestDatabase;

	private void Awake()
	{
		if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;

		itemDatabase = new ItemDatabase(itemTableFileName);
		chestDatabase = new ChestDropDatabase(chestTableFileName);
	}

	// ✅ 宝箱掉落，按权重随机，数量随机区间
	public List<DropResult> GetDropByChestId(int chestId)
	{
		var chestData = chestDatabase.GetChestById(chestId);
		if (chestData == null)
		{
			Debug.LogWarning($"没有找到宝箱数据，ID: {chestId}");
			return null;
		}

		int[] weights = chestData.parsedWeights;
		int[] itemTypes = chestData.parsedItemTypes;
		int[][] amountRanges = chestData.parsedAmountRanges;

		if (weights == null || itemTypes == null || amountRanges == null
			|| weights.Length == 0 || itemTypes.Length == 0 || amountRanges.Length == 0)
		{
			Debug.LogWarning("宝箱数据解析异常");
			return null;
		}

		int selectedIndex = GetRandomIndexByWeight(weights);
		int itemType = itemTypes[selectedIndex];

		int[] range = amountRanges[selectedIndex];
		int amount = UnityEngine.Random.Range(range[0], range[1] + 1);

		var drop = GetRandomDropByItemType(itemType, amount);
		if (drop == null)
		{
			Debug.LogWarning($"物品类型{itemType}没有对应物品");
			return null;
		}

		return new List<DropResult> { drop };
	}

	// ✅ 按物品类型随机掉落（用item.weight作为权重）
	public DropResult GetRandomDropByItemType(int itemType, int amount = 1)
	{
		var items = itemDatabase.GetItemsByType(itemType);
		if (items == null || items.Count == 0) return null;

		int totalWeight = items.Sum(i => i.weight);
		int rand = UnityEngine.Random.Range(0, totalWeight);
		int acc = 0;

		foreach (var item in items)
		{
			acc += item.weight;
			if (rand < acc)
			{
				return new DropResult
				{
					itemData = item,
					amount = amount
				};
			}
		}

		// 兜底返回最后一个
		return new DropResult { itemData = items.Last(), amount = amount };
	}

	private int GetRandomIndexByWeight(int[] weights)
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

	public class DropResult
	{
		public ItemDatabase.ItemData itemData;
		public int amount;
	}
}
