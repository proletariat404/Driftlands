using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEventUI : MonoBehaviour
{
	public static GameEventUI Instance { get; private set; }

	[Header("UI Elements")]
	public GameObject panel;
	public Text titleText;
	public Text descriptionText;
	public Transform buttonContainer;
	public GameObject optionButtonPrefab;

	public Action<EventBehaviorDatabase.BehaviorData> onBehaviorSelected;

	private PlayerStats playerStats;
	private GameEventBehaviorHandler behaviorHandler;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		panel.SetActive(false);

		playerStats = FindObjectOfType<PlayerStats>();

		// �ҵ��򴴽���Ϊ������
		behaviorHandler = FindObjectOfType<GameEventBehaviorHandler>();
		if (behaviorHandler == null)
		{
			GameObject handlerObj = new GameObject("GameEventBehaviorHandler");
			behaviorHandler = handlerObj.AddComponent<GameEventBehaviorHandler>();
		}
	}

	public void Show(GameEventsDatabase.GameEventData data, EventBehaviorDatabase eventBehaviorDb)
	{
		panel.SetActive(true);
		ClearButtons();

		titleText.text = data.title;
		descriptionText.text = data.description;

		var behaviors = eventBehaviorDb.GetBehaviorsByEventId(data.event_id);
		foreach (var behavior in behaviors)
		{
			if (!ConditionChecker.IsConditionMet(behavior.condition))
				continue;

			GameObject btnObj = Instantiate(optionButtonPrefab, buttonContainer);
			var btn = btnObj.GetComponent<Button>();
			var txt = btnObj.GetComponentInChildren<Text>();

			if (behavior.display_text == "ս��")
			{
				txt.text = behavior.display_text;
			}
			else
			{
				float spiritualityBonus = playerStats != null ? playerStats.GetSpirituality() * 0.01f : 0f;
				float finalRate = Mathf.Clamp01(behavior.rate + spiritualityBonus);
				string rateStr = FormatRateWithColor(finalRate);
				txt.text = $"{behavior.display_text} ({rateStr})";
			}

			var behaviorCopy = behavior;
			btn.onClick.AddListener(() =>
			{
				Hide();

				// ֱ�Ӵ�����Ϊ��������ͨ���¼�
				behaviorHandler.ExecuteBehavior(behaviorCopy);

				// ����������ط���Ҫ������Ҳ���Դ����¼�
				onBehaviorSelected?.Invoke(behaviorCopy);
			});
		}
	}

	private string FormatRateWithColor(float rate)
	{
		string color;
		if (rate >= 0.7f) color = "green";
		else if (rate >= 0.4f) color = "yellow";
		else color = "red";
		return $"<color={color}>{(rate * 100f):F0}%</color>";
	}

	public void Hide()
	{
		panel.SetActive(false);
		ClearButtons();
		onBehaviorSelected = null;
	}

	private void ClearButtons()
	{
		foreach (Transform child in buttonContainer)
		{
			Destroy(child.gameObject);
		}
	}
}