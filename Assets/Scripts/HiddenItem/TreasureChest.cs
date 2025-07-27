using UnityEngine;

public class TreasureChest : Interactable
{
	public int chestId = 1;  // ���ñ���ı���ID

	protected override void OnInteract(GameObject player)
	{
		Debug.Log("����򿪣���ʼ������Ʒ");

		var drops = DropManager.Instance.GetDropByChestId(chestId);
		if (drops == null || drops.Count == 0)
		{
			Debug.LogWarning("û���ҵ���������");
			return;
		}

		foreach (var drop in drops)
		{
			// �����ȼ������Ҳ���Ե��ñ�������߼�
			Debug.Log($"���{drop.itemData.type_name}��{drop.itemData.item_name} �� {drop.amount}");

			// ���� UI ��ʾ��ʾ
			UIHintManager.Instance?.ShowHint($"���{drop.itemData.type_name}��{drop.itemData.item_name} �� {drop.amount}");
		}

		// ���俪����������Ч���Է�����

		Destroy(gameObject); // ��������
	}
}
