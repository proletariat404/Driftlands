using TMPro;   // �������TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class RandomEventUI : MonoBehaviour
{
	public GameObject eventPanel;
	public TMP_Text eventDescription; // �������ͨText���� Text ����
	public Button closeButton;

	[Header("���ڳ���ʱ������")]
	[SerializeField]
	private float displayDuration = 5f;  // �Զ��ر�ʱ�䣬��λ����
	private float timer = 0f;
	private bool isShowing = false;

	private void Awake()
	{
		if (closeButton != null)
			closeButton.onClick.AddListener(HideEvent);

		HideEvent();  // ��ʼʱ�����¼����
	}

	private void Update()
	{
		if (isShowing)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				HideEvent();  // ����ָ��ʱ���Զ�����
			}
		}
	}

	public void ShowEvent(string description)
	{
		if (eventPanel == null || eventDescription == null) return;

		eventDescription.text = description;
		eventPanel.SetActive(true);
		isShowing = true;
		timer = displayDuration;  // ���ü�ʱ��
	}

	public void HideEvent()
	{
		if (eventPanel == null) return;

		eventPanel.SetActive(false);
		isShowing = false;
	}
}
