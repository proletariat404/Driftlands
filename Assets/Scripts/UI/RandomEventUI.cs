using UnityEngine;
using UnityEngine.UI; // ���� Unity UI �������ռ�

public class RandomEventUI : MonoBehaviour
{
	[Header("UI Elements")]
	public GameObject eventPanel;
	public Text eventDescription; // �� TMP_Text ��Ϊ Text
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
	/// ��ʾ�¼���ʹ�������ı�������
	/// </summary>
	public void ShowEvent(string description)
	{
		if (eventPanel == null || eventDescription == null) return;

		eventDescription.text = description; // ʹ����ͨ Text �������¼�����
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
