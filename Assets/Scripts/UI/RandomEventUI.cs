using TMPro;   // 如果你用TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class RandomEventUI : MonoBehaviour
{
	public GameObject eventPanel;
	public TMP_Text eventDescription; // 如果用普通Text换成 Text 类型
	public Button closeButton;

	[Header("窗口持续时间设置")]
	[SerializeField]
	private float displayDuration = 5f;  // 自动关闭时间，单位是秒
	private float timer = 0f;
	private bool isShowing = false;

	private void Awake()
	{
		if (closeButton != null)
			closeButton.onClick.AddListener(HideEvent);

		HideEvent();  // 初始时隐藏事件面板
	}

	private void Update()
	{
		if (isShowing)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				HideEvent();  // 超过指定时间自动隐藏
			}
		}
	}

	public void ShowEvent(string description)
	{
		if (eventPanel == null || eventDescription == null) return;

		eventDescription.text = description;
		eventPanel.SetActive(true);
		isShowing = true;
		timer = displayDuration;  // 重置计时器
	}

	public void HideEvent()
	{
		if (eventPanel == null) return;

		eventPanel.SetActive(false);
		isShowing = false;
	}
}
