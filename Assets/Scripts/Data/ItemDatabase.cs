using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ItemDatabase
{
    private Dictionary<int, ItemData> items = new();

    public ItemDatabase(string fileName)
    {
        LoadItemsFromFile(fileName);
    }

    private void LoadItemsFromFile(string fileName)
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileName);
        Debug.Log($"[ItemDatabase] ���Լ�����Ʒ�ļ�: {path}");

        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                Debug.Log($"[ItemDatabase] ��ȡJSON���ݳ���: {json.Length}");

                ItemList list = JsonUtility.FromJson<ItemList>("{\"items\":" + json + "}");

                if (list?.items != null)
                {
                    foreach (var item in list.items)
                    {
                        items[item.item_id] = item;
                        Debug.Log($"[ItemDatabase] ������Ʒ: ID={item.item_id}, Name={item.item_name}, Type={item.type_name}");
                    }
                    Debug.Log($"[ItemDatabase] �ɹ����� {items.Count} ����Ʒ");
                }
                else
                {
                    Debug.LogError("[ItemDatabase] ����ʧ��: list �� items Ϊ��");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ItemDatabase] ���س���: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError($"[ItemDatabase] �ļ�������: {path}");
        }
    }

    public ItemData GetItemById(int id)
    {
        bool found = items.TryGetValue(id, out var item);
        Debug.Log($"[ItemDatabase] ��ѯ��ƷID {id}: {(found ? $"�ҵ� {item.item_name}" : "δ�ҵ�")}");

        if (!found)
        {
            Debug.Log($"[ItemDatabase] ��ǰ�Ѽ��ص���ƷID: {string.Join(", ", items.Keys)}");
        }

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

    // ���һЩʵ�÷���
    public ItemData GetItemByTypeAndId(int type, int id)
    {
        return items.Values.FirstOrDefault(i => i.item_type == type && i.item_id == id);
    }

    public int GetItemCount()
    {
        return items.Count;
    }

    public bool HasItem(int id)
    {
        return items.ContainsKey(id);
    }

    [System.Serializable]
    public class ItemData
    {
        public int item_id;
        public string item_name;
        public int item_type;
        public string type_name;
        public int weight;

        public override string ToString()
        {
            return $"ItemData(ID:{item_id}, Name:{item_name}, Type:{type_name})";
        }
    }
    public string GetItemName(int itemId)
    {
        var item = GetItemById(itemId);
        return item != null ? item.item_name : $"δ֪��Ʒ({itemId})";
    }


    [System.Serializable]
    private class ItemList
    {
        public List<ItemData> items;
    }
}