using UnityEngine;
using UnityEngine.UI; // 引入 Unity UI 的命名空间

public class RandomEventUI : MonoBehaviour
{
	[Header("UI Elements")]
	public GameObject eventPanel;
	public Text eventDescription; // 将 TMP_Text 改为 Text
	public Button closeButton;

	[Header("Configuration")]
	[SerializeField] private float displayDuration = 5f;

	private float timer = 0f;
	private bool isShowing = false;

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
			if (timer <= 0f) HideEvent();
		}
	}

	/// <summary>
	/// 显示事件（使用完整文本描述）
	/// </summary>
	public void ShowEvent(string description)
	{
		if (eventPanel == null || eventDescription == null) return;

		eventDescription.text = description; // 使用普通 Text 来设置事件描述
		eventPanel.SetActive(true);
		isShowing = true;
		timer = displayDuration;
	}

	public void HideEvent()
	{
		if (eventPanel == null) return;

		eventPanel.SetActive(false);
		isShowing = false;
	}
}
