using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("����")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaPerUnit = 1f;

    [Header("��֪��")]
    public int perception = 1;

    [Header("����")]
    public int spirituality = 1;

    [Header("����")]
    public int luck = 1;

    [Header("����Ĭ���츳�������״�ѡ��")]
    public float defaultHuangDiStaminaBonus = 5f;
    public float defaultHuangDiNoStaminaChance = 0.15f;
    public int defaultYanDiPerceptionBonus = 5;
    public float defaultYanDiExtraRewardChance = 0.2f;
    public int defaultChiYouSpiritualityBonus = 5;

    [Header("�����츳��Ϣ")]
    [SerializeField] public TribeType selectedTribe = TribeType.None;

    [SerializeField, Range(0f, 1f), Tooltip("�������ƶ����� - �Ƶ۲����츳")]
    public float noStaminaMovementChance = 0f;

    [SerializeField, Range(0f, 1f), Tooltip("����ɼ��������� - �׵۲����츳")]
    public float extraGatherRewardChance = 0f;

    void Awake()
    {
        currentStamina = maxStamina;
    }

    #region ������ط���

    /// <summary>
    /// ����������������������۹��ܣ�
    /// �����Ƿ�ɹ���������۳ɹ��Ϳ������ɹ���
    /// </summary>
    public bool TryConsumeStamina(float amount)
    {
        if (selectedTribe == TribeType.HuangDi)
        {
            float rand = UnityEngine.Random.Range(0f, 1f);
            if (rand < noStaminaMovementChance)
            {
                Debug.Log($"����������������ֵ={rand:F2} < ����={noStaminaMovementChance}");
                return true;  // �����������Ϊ�ɹ�
            }
        }

        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            Debug.Log($"�۳����� {amount}, ʣ�� {currentStamina}");
            return true;
        }
        else
        {
            Debug.LogWarning("�������㣬�۳�ʧ��");
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
    /// ���ò�����Ϣ���� TribeSystem ���ã�
    /// </summary>
    public void SetTribeInfo(TribeType tribeType, float moveChance, float gatherChance)
    {
        selectedTribe = tribeType;
        noStaminaMovementChance = moveChance;
        extraGatherRewardChance = gatherChance;
    }

    #endregion

    #region �������

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

    #region �罻���¼����ʵȣ��ԣ����ֲ��䣩

    // ... ��֮ǰ���罻���¼����뱣�ֲ��� ...

    #endregion

    #region ������ط������� TribeSystem ���ã�

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
                Debug.LogWarning("δ֪�������ͣ�δӦ�üӳ�");
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
