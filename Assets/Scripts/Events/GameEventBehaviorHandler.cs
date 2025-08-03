using System.Collections.Generic;
using UnityEngine;

public class GameEventBehaviorHandler : MonoBehaviour
{
	private PlayerStats playerStats;

	private void Awake()
	{
		playerStats = FindObjectOfType<PlayerStats>();
		if (playerStats == null)
			Debug.LogError("�Ҳ��� PlayerStats");
	}

	public void ExecuteBehavior(EventBehaviorDatabase.BehaviorData behavior)
	{
		if (behavior == null)
		{
			Debug.LogWarning("��Ϊ����Ϊ��");
			return;
		}

		bool conditionMet = string.IsNullOrEmpty(behavior.condition) || ConditionChecker.IsConditionMet(behavior.condition);
		if (!conditionMet)
		{
			Debug.Log($"��������Ϊ����: {behavior.display_text} ����Ϊ {behavior.condition}");
			return;
		}

		float baseRate = behavior.rate;
		float spiritualityBonus = playerStats.GetSpirituality() * 0.01f;
		float finalRate = Mathf.Clamp01(baseRate + spiritualityBonus);

		float rand = Random.Range(0f, 1f);
		bool success = rand <= finalRate;

		Debug.Log($"ִ����Ϊ: {behavior.display_text} �����ɹ���={baseRate:P1} ���Լӳ�={spiritualityBonus:P1} ���ճɹ���={finalRate:P1} ���ֵ={rand:F2} ���={(success ? "�ɹ�" : "ʧ��")}");

		if (success)
		{
			GrantRewards(behavior.reward_id);
			GrantRewards(behavior.extra_reward_id);
			Debug.Log("�ɹ�����ý�����");
		}
		else
		{
			ShowPenalty(behavior.penalty_id);
			Debug.Log("ʧ�ܣ���������������αս����");
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
						Debug.Log($"������ƷID {itemId} x{amount}");
						// TODO: ������Ʒϵͳ�����Ʒ
					}
				}
			}
		}
		catch
		{
			Debug.LogWarning($"��������ʧ��: {rewardJson}");
		}
	}

	private void ShowPenalty(string penaltyText)
	{
		if (string.IsNullOrEmpty(penaltyText))
			return;

		Debug.Log($"�ͷ�Ч��: {penaltyText}");
		// TODO: ��ʾ UI �ͷ���ʾ
	}

	[System.Serializable]
	private class IntArrayWrapper
	{
		public int[][] array;
	}
}
