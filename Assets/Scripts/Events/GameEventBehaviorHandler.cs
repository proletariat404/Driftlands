using System.Collections.Generic;
using UnityEngine;

public class GameEventBehaviorHandler : MonoBehaviour
{
	private PlayerStats playerStats;

	private void Awake()
	{
		playerStats = FindObjectOfType<PlayerStats>();
		if (playerStats == null)
			Debug.LogError("找不到 PlayerStats");
	}

	public void ExecuteBehavior(EventBehaviorDatabase.BehaviorData behavior)
	{
		if (behavior == null)
		{
			Debug.LogWarning("行为数据为空");
			return;
		}

		bool conditionMet = string.IsNullOrEmpty(behavior.condition) || ConditionChecker.IsConditionMet(behavior.condition);
		if (!conditionMet)
		{
			Debug.Log($"不满足行为条件: {behavior.display_text} 条件为 {behavior.condition}");
			return;
		}

		float baseRate = behavior.rate;
		float spiritualityBonus = playerStats.GetSpirituality() * 0.01f;
		float finalRate = Mathf.Clamp01(baseRate + spiritualityBonus);

		float rand = Random.Range(0f, 1f);
		bool success = rand <= finalRate;

		Debug.Log($"执行行为: {behavior.display_text} 基础成功率={baseRate:P1} 灵性加成={spiritualityBonus:P1} 最终成功率={finalRate:P1} 随机值={rand:F2} 结果={(success ? "成功" : "失败")}");

		if (success)
		{
			GrantRewards(behavior.reward_id);
			GrantRewards(behavior.extra_reward_id);
			Debug.Log("成功！获得奖励。");
		}
		else
		{
			ShowPenalty(behavior.penalty_id);
			Debug.Log("失败，怪物生气，进入伪战斗！");
		}
	}

	private void GrantRewards(string rewardJson)
	{
		if (string.IsNullOrEmpty(rewardJson))
			return;

		try
		{
			var rewards = JsonUtility.FromJson<IntArrayWrapper>("{\"array\":" + rewardJson + "}");
			if (rewards?.array != null)
			{
				foreach (var pair in rewards.array)
				{
					if (pair.Length >= 2)
					{
						int itemId = pair[0];
						int amount = pair[1];
						Debug.Log($"奖励物品ID {itemId} x{amount}");
						// TODO: 调用物品系统添加物品
					}
				}
			}
		}
		catch
		{
			Debug.LogWarning($"解析奖励失败: {rewardJson}");
		}
	}

	private void ShowPenalty(string penaltyText)
	{
		if (string.IsNullOrEmpty(penaltyText))
			return;

		Debug.Log($"惩罚效果: {penaltyText}");
		// TODO: 显示 UI 惩罚提示
	}

	[System.Serializable]
	private class IntArrayWrapper
	{
		public int[][] array;
	}
}
