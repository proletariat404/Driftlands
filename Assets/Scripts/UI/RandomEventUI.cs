using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomEventUI : MonoBehaviour
{
	public GameObject eventPanel;
	public TMP_Text eventDescription;
	public Button closeButton;

	[Header("窗口持续显示时间设置")]
	[SerializeField]
	private float displayDuration = 5f;
	private float timer = 0f;
	private bool isShowing = false;

	private ItemDatabase itemDb; // 添加物品数据库引用

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
	/// 设置物品数据库引用
	/// </summary>
	public void SetItemDatabase(ItemDatabase database)
	{
		itemDb = database;
	}

	/// <summary>
	/// 显示事件（使用原始描述文本）
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
	/// 显示事件（使用事件数据，自动生成奖励/惩罚信息）
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
	/// 生成包含奖励/惩罚信息的完整事件文本
	/// </summary>
	private string GenerateFullEventText(RandomEventsDatabase.RandomEventData eventData)
	{
		string fullText = eventData.event_text;

		// 添加分隔线
		fullText += "\n" + new string('-', 30);

		// 处理奖励
		if (eventData.reward_item_id > 0 && eventData.reward_amount > 0)
		{
			var rewardItem = itemDb?.GetItemById(eventData.reward_item_id);
			if (rewardItem != null)
			{
				fullText += $"\n<color=green>【奖励】获得 {rewardItem.item_name} x{eventData.reward_amount}</color>";
			}
			else
			{
				fullText += $"\n<color=green>【奖励】获得未知物品(ID:{eventData.reward_item_id}) x{eventData.reward_amount}</color>";
			}
		}

		// 处理惩罚
		if (!string.IsNullOrEmpty(eventData.penalty_id) && eventData.penalty_id != "0")
		{
			fullText += $"\n<color=red>【惩罚】{GetPenaltyDescription(eventData.penalty_id)}</color>";
		}

		// 如果既没有奖励也没有惩罚
		if ((eventData.reward_item_id <= 0 || eventData.reward_amount <= 0) &&
			(string.IsNullOrEmpty(eventData.penalty_id) || eventData.penalty_id == "0"))
		{
			fullText += "\n<color=gray>【结果】无特殊效果</color>";
		}

		return fullText;
	}

	/// <summary>
	/// 根据惩罚ID获取惩罚描述
	/// </summary>
	private string GetPenaltyDescription(string penaltyId)
	{
		switch (penaltyId.ToLower())
		{
			case "money_loss":
				return "损失金钱";
			case "health_loss":
				return "损失生命值";
			case "energy_loss":
				return "损失体力";
			case "item_loss":
				return "丢失物品";
			case "time_loss":
				return "浪费时间";
			case "reputation_loss":
				return "声望下降";
			default:
				return $"惩罚效果: {penaltyId}";
		}
	}

	public void HideEvent()
	{
		if (eventPanel == null) return;

		eventPanel.SetActive(false);
		isShowing = false;
	}
}