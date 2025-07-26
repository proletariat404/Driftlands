using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("体力设定")]
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float staminaPerUnit = 1f;

    [Header("感知属性")]
    public int perception = 1; // 每 1 点 = 1% 概率

    void Awake()
    {
        currentStamina = maxStamina;
    }

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

    public int GetPerception()
    {
        return perception;
    }
}
