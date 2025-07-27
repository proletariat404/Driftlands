using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	[Header("体力")]
	public float maxStamina = 100f;
	public float currentStamina = 100f;
	public float staminaPerUnit = 1f;

	[Header("感知力")]
	public int perception = 1;

	[Header("灵性")]
	public int spirituality = 1;

	[Header("幸运")]
	public int luck = 1;

	[Header("部族信息")]
	public TribeType selectedTribe = TribeType.None;
	public float noStaminaMovementChance = 0f;
	public float extraGatherRewardChance = 0f;

	void Awake()
	{
		currentStamina = maxStamina;
	}

	#region 体力相关方法
	public bool ConsumeStamina(float amount)
	{
		if (currentStamina >= amount)
		{
			currentStamina -= amount;
			return true;
		}
		return false;
	}

	public void RecoverStamina(float amount)
	{
		currentStamina = Mathf.Min(currentStamina + amount, maxStamina);
	}

	public bool IsStaminaDepleted()
	{
		return currentStamina <= 0f;
	}

	public void SetStamina(float value)
	{
		currentStamina = Mathf.Clamp(value, 0f, maxStamina);
	}

	/// <summary>
	/// 设置部族信息（由 TribeSystem 调用）
	/// </summary>
	public void SetTribeInfo(TribeType tribeType, float moveChance, float gatherChance)
	{
		selectedTribe = tribeType;
		noStaminaMovementChance = moveChance;
		extraGatherRewardChance = gatherChance;
	}

	/// <summary>
	/// 检查移动是否消耗体力
	/// </summary>
	public bool ShouldConsumeStaminaForMovement()
	{
		if (selectedTribe != TribeType.HuangDi) return true;
		return UnityEngine.Random.Range(0f, 1f) > noStaminaMovementChance;
	}

	/// <summary>
	/// 检查采集是否获得额外奖励
	/// </summary>
	public bool ShouldGetExtraGatherReward()
	{
		if (selectedTribe != TribeType.YanDi) return false;
		return UnityEngine.Random.Range(0f, 1f) <= extraGatherRewardChance;
	}

	/// <summary>
	/// 检查是否可以与异兽交流
	/// </summary>
	public bool CanCommunicateWithBeasts()
	{
		return selectedTribe == TribeType.ChiYou;
	}
	#endregion

	#region 属性获取方法
	public int GetPerception()
	{
		return perception;
	}

	public int GetSpirituality()
	{
		return spirituality;
	}

	public int GetLuck()
	{
		return luck;
	}
	#endregion

	#region 社交影响力计算（灵性相关）
	public float CalculateSocialSuccessRate(float baseSuccessRate)
	{
		float spiritualityBonus = spirituality * 0.01f;
		return Mathf.Clamp01(baseSuccessRate + spiritualityBonus);
	}

	public bool PerformSocialAction(float baseSuccessRate)
	{
		float finalRate = CalculateSocialSuccessRate(baseSuccessRate);
		return UnityEngine.Random.Range(0f, 1f) <= finalRate;
	}
	#endregion

	#region 随机事件概率计算（幸运相关）
	public enum EventType
	{
		Good,
		Normal,
		Bad
	}

	public float[] CalculateEventWeights(float goodWeight, float normalWeight, float badWeight)
	{
		float luckModifier = luck * 0.02f;

		float modifiedGoodWeight = goodWeight * (1f + luckModifier);
		float modifiedNormalWeight = normalWeight;
		float modifiedBadWeight = badWeight * (1f - luckModifier);

		modifiedBadWeight = Mathf.Max(modifiedBadWeight, 0.1f);

		return new float[] { modifiedGoodWeight, modifiedNormalWeight, modifiedBadWeight };
	}

	public EventType GetRandomEventType(float goodWeight, float normalWeight, float badWeight)
	{
		float[] weights = CalculateEventWeights(goodWeight, normalWeight, badWeight);
		float totalWeight = weights[0] + weights[1] + weights[2];
		float randomValue = UnityEngine.Random.Range(0f, totalWeight);

		if (randomValue < weights[0])
			return EventType.Good;
		else if (randomValue < weights[0] + weights[1])
			return EventType.Normal;
		else
			return EventType.Bad;
	}

	public bool ShouldTriggerRandomEvent(float baseEventChance = 0.5f)
	{
		return UnityEngine.Random.Range(0f, 1f) <= baseEventChance;
	}
	#endregion

	#region 属性设置方法
	public void SetSpirituality(int value)
	{
		spirituality = Mathf.Max(0, value);
	}

	public void SetLuck(int value)
	{
		luck = Mathf.Max(0, value);
	}

	public void ModifySpirituality(int amount)
	{
		spirituality = Mathf.Max(0, spirituality + amount);
	}

	public void ModifyLuck(int amount)
	{
		luck = Mathf.Max(0, luck + amount);
	}
	#endregion

	#region 部族相关方法（供 TribeSystem 调用）

	/// <summary>
	/// 是否已经选择了部族
	/// </summary>
	public bool HasSelectedTribe()
	{
		return selectedTribe != TribeType.None;
	}

	/// <summary>
	/// 应用选择的部族，并设置加成
	/// </summary>
	public void ApplyTribeSelection(TribeType tribeType)
	{
		switch (tribeType)
		{
			case TribeType.HuangDi:
				maxStamina += 5f;
				SetTribeInfo(TribeType.HuangDi, 0.15f, 0f);
				break;
			case TribeType.YanDi:
				perception += 5;
				SetTribeInfo(TribeType.YanDi, 0f, 0.2f);
				break;
			case TribeType.ChiYou:
				spirituality += 5;
				SetTribeInfo(TribeType.ChiYou, 0f, 0f);
				break;
			default:
				Debug.LogWarning("未知部族类型，未应用加成");
				break;
		}
	}

	/// <summary>
	/// 重置部族选择（例如重新选择时使用）
	/// </summary>
	public void ResetTribeSelection()
	{
		selectedTribe = TribeType.None;
		noStaminaMovementChance = 0f;
		extraGatherRewardChance = 0f;
	}

	#endregion
}
