using UnityEngine;

public class TreasureChest : Interactable
{
	public int chestId = 1;  // 配置表里的宝箱ID

	protected override void OnInteract(GameObject player)
	{
		Debug.Log("宝箱打开，开始掉落物品");

		var drops = DropManager.Instance.GetDropByChestId(chestId);
		if (drops == null || drops.Count == 0)
		{
			Debug.LogWarning("没有找到掉落配置");
			return;
		}

		foreach (var drop in drops)
		{
			// 这里先简单输出，也可以调用背包添加逻辑
			Debug.Log($"获得{drop.itemData.type_name}：{drop.itemData.item_name} × {drop.amount}");

			// 调用 UI 提示显示
			UIHintManager.Instance?.ShowHint($"获得{drop.itemData.type_name}：{drop.itemData.item_name} × {drop.amount}");
		}

		// 宝箱开启动画或特效可以放这里

		Destroy(gameObject); // 开完销毁
	}
}
