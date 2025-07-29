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

    [Header("部族默认天赋（用于首次选择）")]
    public float defaultHuangDiStaminaBonus = 5f;
    public float defaultHuangDiNoStaminaChance = 0.15f;
    public int defaultYanDiPerceptionBonus = 5;
    public float defaultYanDiExtraRewardChance = 0.2f;
    public float defaultYanDiExtraRewardMultiplier = 1.2f;  // 默认倍率1.2倍
    public int defaultChiYouSpiritualityBonus = 5;

    [Header("部族天赋信息")]
    public TribeType selectedTribe = TribeType.None;

    [Header("黄帝天赋 - 免体力移动概率")]
    [Range(0f, 1f)]
    public float noStaminaMovementChance = 0f;

    [Header("炎帝天赋 - 额外采集奖励概率")]
    [Range(0f, 1f)]
    public float extraGatherRewardChance = 0f;

    [Header("炎帝天赋 - 额外采集奖励倍率")]
    [Range(1f, 10f)]
    public float extraGatherRewardMultiplier = 1.0f;

    void Awake()
    {
        currentStamina = maxStamina;
    }

    #region 体力相关方法

    public bool TryConsumeStamina(float amount)
    {
        if (selectedTribe == TribeType.HuangDi)
        {
            float rand = Random.Range(0f, 1f);
            if (rand < noStaminaMovementChance)
            {
                Debug.Log($"免扣体力触发，随机值={rand:F2} < 概率={noStaminaMovementChance}");
                return true;  // 免扣体力，视为成功
            }
        }

        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            Debug.Log($"扣除体力 {amount}, 剩余 {currentStamina}");
            return true;
        }
        else
        {
            Debug.LogWarning("体力不足，扣除失败");
            return false;
        }
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

    #endregion

    #region 属性相关

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

    #region 部族相关方法（供 TribeSystem 调用）

    public bool HasSelectedTribe()
    {
        return selectedTribe != TribeType.None;
    }

    public void ApplyTribeSelection(TribeType tribeType)
    {
        selectedTribe = tribeType;

        switch (tribeType)
        {
            case TribeType.HuangDi:
                maxStamina += defaultHuangDiStaminaBonus;
                currentStamina += defaultHuangDiStaminaBonus;
                if (noStaminaMovementChance <= 0f)
                    noStaminaMovementChance = defaultHuangDiNoStaminaChance;
                break;

            case TribeType.YanDi:
                perception += defaultYanDiPerceptionBonus;
                if (extraGatherRewardChance <= 0f)
                    extraGatherRewardChance = defaultYanDiExtraRewardChance;
                if (extraGatherRewardMultiplier <= 1f)
                    extraGatherRewardMultiplier = defaultYanDiExtraRewardMultiplier;
                break;

            case TribeType.ChiYou:
                spirituality += defaultChiYouSpiritualityBonus;
                break;

            default:
                Debug.LogWarning("未知部族类型，未应用加成");
                break;
        }
    }

    public void ResetTribeSelection()
    {
        selectedTribe = TribeType.None;
        noStaminaMovementChance = 0f;
        extraGatherRewardChance = 0f;
        extraGatherRewardMultiplier = 1f;
    }

    #endregion
}
