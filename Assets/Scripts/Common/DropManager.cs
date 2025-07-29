using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance { get; private set; }

    public string itemTableFileName = "item.json";
    public string chestTableFileName = "chest.json";
    public string resourceTableFileName = "resource.json";

    private ItemDatabase itemDatabase;
    private ChestDropDatabase chestDatabase;
    private ResourceDropDatabase resourceDatabase;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        itemDatabase = new ItemDatabase(itemTableFileName);
        chestDatabase = new ChestDropDatabase(chestTableFileName);
        resourceDatabase = new ResourceDropDatabase(resourceTableFileName);
    }

    #region 宝箱掉落逻辑
    public List<DropResult> GetDropByChestId(int chestId)
    {
        var chestData = chestDatabase.GetChestById(chestId);
        if (chestData == null)
        {
            Debug.LogWarning($"没有找到宝箱数据，ID: {chestId}");
            return null;
        }

        return ProcessChestDrop(chestData);
    }

    /// <summary>
    /// 根据宝箱类型或宝箱ID获取掉落，优先用宝箱ID
    /// </summary>
    /// <param name="chestType">宝箱类型，填0表示无效</param>
    /// <param name="chestId">宝箱ID，填0表示无效</param>
    /// <returns>掉落列表</returns>
    public List<DropResult> GetDropByChestTypeOrId(int chestType, int chestId)
    {
        ChestDropDatabase.ChestDropData chestData = null;

        if (chestId > 0)
        {
            // 优先使用ID
            chestData = chestDatabase.GetChestById(chestId);
            if (chestData == null)
            {
                Debug.LogWarning($"没有找到宝箱数据，ID: {chestId}");
                return null;
            }
        }
        else if (chestType > 0)
        {
            // 使用类型随机选择
            var chestList = chestDatabase.GetChestsByType(chestType);
            if (chestList == null || chestList.Count == 0)
            {
                Debug.LogWarning($"没有找到类型为{chestType}的宝箱数据");
                return null;
            }
            int index = UnityEngine.Random.Range(0, chestList.Count);
            chestData = chestList[index];
        }
        else
        {
            Debug.LogWarning("宝箱类型和ID均无效，无法获取宝箱掉落");
            return null;
        }

        return ProcessChestDrop(chestData);
    }

    /// <summary>
    /// 处理宝箱掉落的通用逻辑
    /// </summary>
    private List<DropResult> ProcessChestDrop(ChestDropDatabase.ChestDropData chestData)
    {
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
    #endregion

    #region 资源掉落逻辑（支持多维 item_types）
    public List<DropResult> GetDropByResourceId(int resourceId)
    {
        var resData = resourceDatabase.GetResourceById(resourceId);
        if (resData == null)
        {
            Debug.LogWarning($"没有找到资源数据，ID: {resourceId}");
            return null;
        }

        int[][] itemTypeDefs = resData.parsedItemTypes;
        int[] weights = resData.parsedWeights;
        int[][] amountRanges = resData.parsedAmountRanges;

        if (itemTypeDefs == null || weights == null || amountRanges == null
            || itemTypeDefs.Length == 0 || weights.Length == 0 || amountRanges.Length == 0)
        {
            Debug.LogWarning("资源数据解析异常");
            return null;
        }

        int selectedIndex = GetRandomIndexByWeight(weights);
        int[] typeDef = itemTypeDefs[selectedIndex];
        int[] range = amountRanges[selectedIndex];
        int amount = UnityEngine.Random.Range(range[0], range[1] + 1);

        DropResult drop = null;

        if (typeDef.Length == 2)
        {
            var item = itemDatabase.GetItemByTypeAndId(typeDef[0], typeDef[1]);
            if (item != null)
            {
                drop = new DropResult { itemData = item, amount = amount };
            }
        }
        else if (typeDef.Length == 1)
        {
            drop = GetRandomDropByItemType(typeDef[0], amount);
        }

        if (drop == null)
        {
            Debug.LogWarning("没有找到掉落配置或没有掉落物品");
            return null;
        }

        return new List<DropResult> { drop };
    }
    #endregion

    #region 新增接口 - 资源掉落支持类型或ID任选，优先ID
    /// <summary>
    /// 根据资源类型或资源ID获取掉落，优先用资源ID
    /// </summary>
    /// <param name="resourceType">资源类型，填0表示无效</param>
    /// <param name="resourceId">资源ID，填0表示无效</param>
    /// <returns>掉落列表</returns>
    public List<DropResult> GetDropByResourceTypeOrId(int resourceType, int resourceId)
    {
        ResourceDropDatabase.ResourceData resData = null;

        if (resourceId > 0)
        {
            resData = resourceDatabase.GetResourceById(resourceId);
            if (resData == null)
            {
                Debug.LogWarning($"没有找到资源数据，ID: {resourceId}");
                return null;
            }
        }
        else if (resourceType > 0)
        {
            var resList = resourceDatabase.GetResourcesByType(resourceType);
            if (resList == null || resList.Count == 0)
            {
                Debug.LogWarning($"没有找到类型为{resourceType}的资源数据");
                return null;
            }
            int index = UnityEngine.Random.Range(0, resList.Count);
            resData = resList[index];
        }
        else
        {
            Debug.LogWarning("资源类型和ID均无效，无法获取资源掉落");
            return null;
        }

        int[][] itemTypeDefs = resData.parsedItemTypes;
        int[] weights = resData.parsedWeights;
        int[][] amountRanges = resData.parsedAmountRanges;

        if (itemTypeDefs == null || weights == null || amountRanges == null
            || itemTypeDefs.Length == 0 || weights.Length == 0 || amountRanges.Length == 0)
        {
            Debug.LogWarning("资源数据解析异常");
            return null;
        }

        int selectedIndex = GetRandomIndexByWeight(weights);
        int[] typeDef = itemTypeDefs[selectedIndex];
        int[] range = amountRanges[selectedIndex];
        int amount = UnityEngine.Random.Range(range[0], range[1] + 1);

        DropResult drop = null;

        if (typeDef.Length == 2)
        {
            var item = itemDatabase.GetItemByTypeAndId(typeDef[0], typeDef[1]);
            if (item != null)
            {
                drop = new DropResult { itemData = item, amount = amount };
            }
        }
        else if (typeDef.Length == 1)
        {
            drop = GetRandomDropByItemType(typeDef[0], amount);
        }

        if (drop == null)
        {
            Debug.LogWarning("没有找到掉落配置或没有掉落物品");
            return null;
        }

        return new List<DropResult> { drop };
    }
    #endregion

    #region 资源类型获取随机资源ID（不以体力消耗为权重，默认等概率）
    public int? GetRandomResourceIdByType(int resourceType)
    {
        var resources = resourceDatabase.GetResourcesByType(resourceType);
        if (resources == null || resources.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, resources.Count);
        return resources[index].resource_id;
    }
    #endregion

    #region 新增接口 - 获取资源名称
    public string GetResourceNameById(int resourceId)
    {
        var resData = resourceDatabase.GetResourceById(resourceId);
        if (resData != null)
            return resData.resource_type_name;
        else
            return "未知资源";
    }
    #endregion

    #region 新增接口 - 获取采集体力消耗
    public int GetStaminaCostByResourceId(int resourceId)
    {
        var resData = resourceDatabase.GetResourceById(resourceId);
        if (resData != null)
            return resData.stamina_cost;
        else
            return 1; // 默认消耗1点体力
    }
    #endregion

    #region 公共方法

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
    #endregion
}