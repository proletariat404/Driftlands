using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventBehaviorHandler : MonoBehaviour
{
    public void ExecuteBehavior(EventBehaviorDatabase.BehaviorData behavior, float finalRate)
    {
        if (behavior.display_text == "战斗")
        {
            Debug.Log("进入战斗");
            // TODO: 触发战斗系统
            return;
        }

        bool isSuccess = RollSuccess(finalRate);
        Debug.Log($"行为：{behavior.display_text}, 成功率: {finalRate}, 判定结果: {isSuccess}");

        string resultMsg = isSuccess
            ? BuildSuccessText(behavior)
            : BuildFailureText(behavior);

        GameEventUI.Instance.ShowResultText(resultMsg);

        if (isSuccess)
        {
            ApplyReward(behavior.reward_id);
            if (!string.IsNullOrEmpty(behavior.extra_reward_id))
                ApplyReward(behavior.extra_reward_id);
        }
        else
        {
            ApplyPenalty(behavior.penalty_id);
        }
    }

    private bool RollSuccess(float probability)
    {
        return UnityEngine.Random.value < probability;
    }

    private string BuildSuccessText(EventBehaviorDatabase.BehaviorData behavior)
    {
        Debug.Log($"原始 reward_id 字符串: {behavior.reward_id}");
        Debug.Log($"原始 extra_reward_id 字符串: {behavior.extra_reward_id}");

        string rewardDesc = GetItemDesc(behavior.reward_id);
        string extraRewardDesc = string.IsNullOrEmpty(behavior.extra_reward_id) ? "" : GetItemDesc(behavior.extra_reward_id);

        string result = $"<color=green>{behavior.display_text}成功！</color> 获得奖励：{rewardDesc}";

        if (!string.IsNullOrEmpty(extraRewardDesc))
        {
            result += $"，额外奖励：{extraRewardDesc}";
        }

        return result;
    }

    private string BuildFailureText(EventBehaviorDatabase.BehaviorData behavior)
    {
        string penalty = string.IsNullOrEmpty(behavior.penalty_id) ? "失败，未造成影响。" : behavior.penalty_id;
        return $"<color=red>{behavior.display_text}失败！</color> {penalty}";
    }

    private void ApplyReward(string rewardId)
    {
        Debug.Log($"应用奖励：{rewardId}");
        // TODO: 这里写发放奖励的具体逻辑
    }

    private void ApplyPenalty(string penaltyId)
    {
        Debug.Log($"应用惩罚：{penaltyId}");
        // TODO: 这里写处理惩罚的具体逻辑
    }

    private string GetItemDesc(string rewardIdStr)
    {
        if (string.IsNullOrEmpty(rewardIdStr)) return "无";

        try
        {
            // 去除外层引号
            string s = rewardIdStr.Trim();
            if (s.StartsWith("\"") && s.EndsWith("\""))
                s = s.Substring(1, s.Length - 2);

            // 这里调用你自己写的解析函数，将字符串转成 List<int[]>
            List<int[]> rewardList = ParseNestedIntArray(s);

            List<string> rewardStrs = new List<string>();
            foreach (var arr in rewardList)
            {
                if (arr.Length >= 2)
                {
                    int itemId = arr[0];
                    int amount = arr[1];
                    string itemName = GameDataManager.Instance.ItemData.GetItemName(itemId);
                    rewardStrs.Add($"{itemName} x{amount}");
                }
            }
            return string.Join("，", rewardStrs);
        }
        catch (Exception ex)
        {
            Debug.LogError($"奖励解析失败：{ex.Message}");
            return rewardIdStr;
        }
    }

    // 这个是参考你 ChestDropDatabase.ParseRangeArray 写的解析函数
    private List<int[]> ParseNestedIntArray(string s)
    {
        List<int[]> results = new List<int[]>();
        s = s.Trim();
        if (string.IsNullOrEmpty(s) || s.Length < 5)
            return results;

        if (s.StartsWith("[") && s.EndsWith("]"))
            s = s.Substring(1, s.Length - 2);

        int start = -1;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '[')
                start = i;
            else if (s[i] == ']' && start >= 0)
            {
                string sub = s.Substring(start + 1, i - start - 1);
                var parts = sub.Split(',');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0].Trim(), out int first) &&
                    int.TryParse(parts[1].Trim(), out int second))
                {
                    results.Add(new int[] { first, second });
                }
                start = -1;
            }
        }

        return results;
    }


    [Serializable]
    private class ItemArrayWrapper
    {
        public int[][] items;
    }
}
