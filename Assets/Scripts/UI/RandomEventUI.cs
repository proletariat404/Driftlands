using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomEventUI : MonoBehaviour
{
	public GameObject eventPanel;
	public TMP_Text eventDescription;
	public Button closeButton;

	[Header("���ڳ�����ʾʱ������")]
	[SerializeField]
	private float displayDuration = 5f;
	private float timer = 0f;
	private bool isShowing = false;

	private ItemDatabase itemDb; // �����Ʒ���ݿ�����

	private void Awake()
	{
		if (closeButton != null)
			closeButton.onClick.AddListener(HideEvent);

		HideEvent();
	}

	private void Update()
	{
		if (isShowing)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				HideEvent();
			}
		}
	}

	/// <summary>
	/// ������Ʒ���ݿ�����
	/// </summary>
	public void SetItemDatabase(ItemDatabase database)
	{
		itemDb = database;
	}

	/// <summary>
	/// ��ʾ�¼���ʹ��ԭʼ�����ı���
	/// </summary>
	public void ShowEvent(string description)
	{
		if (eventPanel == null || eventDescription == null) return;

		eventDescription.text = description;
		eventPanel.SetActive(true);
		isShowing = true;
		timer = displayDuration;
	}

	/// <summary>
	/// ��ʾ�¼���ʹ���¼����ݣ��Զ����ɽ���/�ͷ���Ϣ��
	/// </summary>
	public void ShowEvent(RandomEventsDatabase.RandomEventData eventData)
	{
		if (eventPanel == null || eventDescription == null || eventData == null) return;

		string fullText = GenerateFullEventText(eventData);
		eventDescription.text = fullText;
		eventPanel.SetActive(true);
		isShowing = true;
		timer = displayDuration;
	}

	/// <summary>
	/// ���ɰ�������/�ͷ���Ϣ�������¼��ı�
	/// </summary>
	private string GenerateFullEventText(RandomEventsDatabase.RandomEventData eventData)
	{
		string fullText = eventData.event_text;

		// ��ӷָ���
		fullText += "\n" + new string('-', 30);

		// ������
		if (eventData.reward_item_id > 0 && eventData.reward_amount > 0)
		{
			var rewardItem = itemDb?.GetItemById(eventData.reward_item_id);
			if (rewardItem != null)
			{
				fullText += $"\n<color=green>����������� {rewardItem.item_name} x{eventData.reward_amount}</color>";
			}
			else
			{
				fullText += $"\n<color=green>�����������δ֪��Ʒ(ID:{eventData.reward_item_id}) x{eventData.reward_amount}</color>";
			}
		}

		// ����ͷ�
		if (!string.IsNullOrEmpty(eventData.penalty_id) && eventData.penalty_id != "0")
		{
			fullText += $"\n<color=red>���ͷ���{GetPenaltyDescription(eventData.penalty_id)}</color>";
		}

		// �����û�н���Ҳû�гͷ�
		if ((eventData.reward_item_id <= 0 || eventData.reward_amount <= 0) &&
			(string.IsNullOrEmpty(eventData.penalty_id) || eventData.penalty_id == "0"))
		{
			fullText += "\n<color=gray>�������������Ч��</color>";
		}

		return fullText;
	}

	/// <summary>
	/// ���ݳͷ�ID��ȡ�ͷ�����
	/// </summary>
	private string GetPenaltyDescription(string penaltyId)
	{
		switch (penaltyId.ToLower())
		{
			case "money_loss":
				return "��ʧ��Ǯ";
			case "health_loss":
				return "��ʧ����ֵ";
			case "energy_loss":
				return "��ʧ����";
			case "item_loss":
				return "��ʧ��Ʒ";
			case "time_loss":
				return "�˷�ʱ��";
			case "reputation_loss":
				return "�����½�";
			default:
				return $"�ͷ�Ч��: {penaltyId}";
		}
	}

	public void HideEvent()
	{
		if (eventPanel == null) return;

		eventPanel.SetActive(false);
		isShowing = false;
	}
}