using UnityEngine;

public class TreasureChest : Interactable
{
    public int chestType = 0;  // �������ͣ���0��ʾ��Ч
    public int chestId = 0;    // ����ID����0��ʾ��Ч

    protected override void OnInteract(GameObject player)
    {
        Debug.Log("����򿪣���ʼ������Ʒ");

        // ʹ���µĽӿڣ�����ID���������
        var drops = DropManager.Instance.GetDropByChestTypeOrId(chestType, chestId);
        if (drops == null || drops.Count == 0)
        {
            Debug.LogWarning("û���ҵ���������");
            return;
        }

        foreach (var drop in drops)
        {
            // ������Ը��ݾ�������Ҳ����ֱ�����ڱ�������߼�
            Debug.Log($"���{drop.itemData.type_name}��{drop.itemData.item_name} �� {drop.amount}");

            // ���� UI ��ʾ��ʾ
            UIHintManager.Instance?.ShowHint($"���{drop.itemData.type_name}��{drop.itemData.item_name} �� {drop.amount}");
        }

        // ���俪����������Ч���Է�����

        Destroy(gameObject); // ��������
    }
}