// TreasureChest.cs
using UnityEngine;

public class TreasureChest : Interactable
{
    protected override void OnInteract(GameObject player)
    {
        Debug.Log("�����Զ��򿪲����轱��");
        // ����д�����������߼���
        Destroy(gameObject);
    }
}
