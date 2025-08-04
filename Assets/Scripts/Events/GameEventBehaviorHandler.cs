using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEventBehaviorHandler : MonoBehaviour
{
    public void ExecuteBehavior(EventBehaviorDatabase.BehaviorData behavior, float finalRate)
    {
        if (behavior.display_text == "ս��")
        {
            Debug.Log("����ս��");
            // TODO: ����ս��ϵͳ
            return;
        }

        bool isSuccess = RollSuccess(finalRate);
        Debug.Log($"��Ϊ��{behavior.display_text}, �ɹ���: {finalRate}, �ж����: {isSuccess}");

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
        Debug.Log($"ԭʼ reward_id �ַ���: {behavior.reward_id}");
        Debug.Log($"ԭʼ extra_reward_id �ַ���: {behavior.extra_reward_id}");

        string rewardDesc = GetItemDesc(behavior.reward_id);
        string extraRewardDesc = string.IsNullOrEmpty(behavior.extra_reward_id) ? "" : GetItemDesc(behavior.extra_reward_id);

        string result = $"<color=green>{behavior.display_text}�ɹ���</color> ��ý�����{rewardDesc}";

        if (!string.IsNullOrEmpty(extraRewardDesc))
        {
            result += $"�����⽱����{extraRewardDesc}";
        }

        return result;
    }

    private string BuildFailureText(EventBehaviorDatabase.BehaviorData behavior)
    {
        string penalty = string.IsNullOrEmpty(behavior.penalty_id) ? "ʧ�ܣ�δ���Ӱ�졣" : behavior.penalty_id;
        return $"<color=red>{behavior.display_text}ʧ�ܣ�</color> {penalty}";
    }

    private void ApplyReward(string rewardId)
    {
        Debug.Log($"Ӧ�ý�����{rewardId}");
        // TODO: ����д���Ž����ľ����߼�
    }

    private void ApplyPenalty(string penaltyId)
    {
        Debug.Log($"Ӧ�óͷ���{penaltyId}");
        // TODO: ����д����ͷ��ľ����߼�
    }

    private string GetItemDesc(string rewardIdStr)
    {
        if (string.IsNullOrEmpty(rewardIdStr)) return "��";

        try
        {
            // ȥ���������
            string s = rewardIdStr.Trim();
            if (s.StartsWith("\"") && s.EndsWith("\""))
                s = s.Substring(1, s.Length - 2);

            // ����������Լ�д�Ľ������������ַ���ת�� List<int[]>
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
            return string.Join("��", rewardStrs);
        }
        catch (Exception ex)
        {
            Debug.LogError($"��������ʧ�ܣ�{ex.Message}");
            return rewardIdStr;
        }
    }

    // ����ǲο��� ChestDropDatabase.ParseRangeArray д�Ľ�������
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
