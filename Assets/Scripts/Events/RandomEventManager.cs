using UnityEngine;
using System;

public class RandomEventManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private RandomEventsWeightDatabase weightDb;
    [SerializeField] private RandomEventsDatabase eventsDb;
    [SerializeField] private ItemDatabase itemDb;

    [Header("UI Reference")]
    [SerializeField] private RandomEventUI eventUI;

    private RandomEventSystem eventSystem;

    public event Action<RandomEventsDatabase.RandomEventData> OnEventTriggered;

    public void SetDatabases(
        RandomEventsWeightDatabase weightDb,
        RandomEventsDatabase eventsDb,
        ItemDatabase itemDb)
    {
        this.weightDb = weightDb;
        this.eventsDb = eventsDb;
        this.itemDb = itemDb;
    }

    public void SetEventUI(RandomEventUI ui)
    {
        this.eventUI = ui;
    }

    private void Start()
    {
        InitializeEventSystem();
    }

    private void InitializeEventSystem()
    {
        if (weightDb == null || eventsDb == null)
        {
            Debug.LogError("Event databases not set!");
            return;
        }

        eventSystem = new RandomEventSystem(weightDb, eventsDb);
        eventSystem.OnEventTriggered += HandleSystemEvent;
    }

    /// <summary>
    /// ���û����������ʣ�0~1��
    /// </summary>
    public void SetBaseTriggerChance(float chance)
    {
        if (eventSystem != null) eventSystem.SetBaseTriggerChance(chance);
    }

    /// <summary>
    /// ��������ֵ����0����Ӱ���������¼�Ȩ��
    /// </summary>
    public void SetLuckValue(int luck)
    {
        if (eventSystem != null) eventSystem.SetLuckValue(luck);
    }

    /// <summary>
    /// ��������¼�
    /// </summary>
    public void TriggerRandomEvent()
    {
        if (eventSystem != null)
        {
            eventSystem.TryTriggerEvent();
        }
    }

    private void HandleSystemEvent(RandomEventsDatabase.RandomEventData eventData)
    {
        // ����UI��ʾ
        if (eventData != null && eventUI != null)
        {
            string description = GenerateEventDescription(eventData);
            eventUI.ShowEvent(description);
        }

        // ת���¼���������
        OnEventTriggered?.Invoke(eventData);
    }

    /// <summary>
    /// ���ɰ�����Ʒ���Ƶ������¼�����
    /// </summary>
    public string GenerateEventDescription(RandomEventsDatabase.RandomEventData eventData)
    {
        if (eventData == null) return "δ�����κ��¼�";

        string description = eventData.event_text;
        description += "\n" + new string('-', 20);

        // ��������
        if (eventData.reward_item_id > 0 && eventData.reward_amount > 0)
        {
            var rewardItem = itemDb?.GetItemById(eventData.reward_item_id);
            string itemName = rewardItem != null ?
                rewardItem.item_name :
                $"δ֪��Ʒ(ID:{eventData.reward_item_id})";

            description += $"\n<color=green>����������� {itemName} �� {eventData.reward_amount}</color>";
        }

        // �ͷ�����
        if (!string.IsNullOrEmpty(eventData.penalty_id) && eventData.penalty_id != "0")
        {
            description += $"\n<color=red>���ͷ���{GetPenaltyDescription(eventData.penalty_id)}</color>";
        }

        // ��Ч�������
        if ((eventData.reward_item_id <= 0 || eventData.reward_amount <= 0) &&
            (string.IsNullOrEmpty(eventData.penalty_id) || eventData.penalty_id == "0"))
        {
            description += "\n<color=gray>�������������Ч��</color>";
        }

        return description;
    }

    private string GetPenaltyDescription(string penaltyId)
    {
        return penaltyId.ToLower() switch
        {
            "money_loss" => "��ʧ��Ǯ",
            "health_loss" => "��ʧ����ֵ",
            "energy_loss" or "stamina_loss" => "��ʧ����",
            "item_loss" => "��ʧ��Ʒ",
            "time_loss" => "�˷�ʱ��",
            "reputation_loss" => "�����½�",
            _ => $"�ͷ�Ч��: {penaltyId}"
        };
    }

    private void OnDestroy()
    {
        if (eventSystem != null)
        {
            eventSystem.OnEventTriggered -= HandleSystemEvent;
        }
    }
}