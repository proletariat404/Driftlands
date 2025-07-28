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
    public int defaultChiYouSpiritualityBonus = 5;

    [Header("部族天赋信息")]
    [SerializeField] public TribeType selectedTribe = TribeType.None;

    [SerializeField, Range(0f, 1f), Tooltip("免体力移动概率 - 黄帝部族天赋")]
    public float noStaminaMovementChance = 0f;

    [SerializeField, Range(0f, 1f), Tooltip("额外采集奖励概率 - 炎帝部族天赋")]
    public float extraGatherRewardChance = 0f;

    void Awake()
    {
        currentStamina = maxStamina;
    }

    #region 体力相关方法

    /// <summary>
    /// 尝试消耗体力（带概率免扣功能）
    /// 返回是否成功（包括免扣成功和扣体力成功）
    /// </summary>
    public bool TryConsumeStamina(float amount)
    {
        if (selectedTribe == TribeType.HuangDi)
        {
            float rand = UnityEngine.Random.Range(0f, 1f);
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

    /// <summary>
    /// 设置部族信息（由 TribeSystem 调用）
    /// </summary>
    public void SetTribeInfo(TribeType tribeType, float moveChance, float gatherChance)
    {
        selectedTribe = tribeType;
        noStaminaMovementChance = moveChance;
        extraGatherRewardChance = gatherChance;
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

    #region 社交与事件概率等（略，保持不变）

    // ... 你之前的社交和事件代码保持不变 ...

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
    }

    #endregion
}
