using UnityEngine;
using UnityEngine.UI; // 引入 Unity UI 的命名空间
using System;

public class UIConfirmManager : MonoBehaviour
{
	public static UIConfirmManager Instance { get; private set; }

	[Header("UI 元素")]
	public CanvasGroup confirmCanvasGroup;
	public Text messageText; // 将 TextMeshProUGUI 改为 Text
	public UnityEngine.UI.Button confirmButton;
	public UnityEngine.UI.Button cancelButton;

	private Action confirmCallback;
	private Action cancelCallback;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		confirmCanvasGroup.alpha = 0f;
		confirmCanvasGroup.blocksRaycasts = false;
		confirmCanvasGroup.interactable = false;

		confirmButton.onClick.AddListener(OnConfirmClicked);
		cancelButton.onClick.AddListener(OnCancelClicked);
	}

	public void ShowConfirm(string message, Action onConfirm, Action onCancel)
	{
		messageText.text = message; // 使用普通 Text 来设置消息
		confirmCallback = onConfirm;
		cancelCallback = onCancel;

		confirmCanvasGroup.alpha = 1f;
		confirmCanvasGroup.blocksRaycasts = true;
		confirmCanvasGroup.interactable = true;

		// 暂停输入
		GameInputManager.Instance?.DisableInput();
	}

	public void Hide()
	{
		confirmCanvasGroup.alpha = 0f;
		confirmCanvasGroup.blocksRaycasts = false;
		confirmCanvasGroup.interactable = false;

		// 恢复输入
		GameInputManager.Instance?.EnableInput();
	}

	private void OnConfirmClicked()
	{
		confirmCallback?.Invoke();
		Hide();
	}

	private void OnCancelClicked()
	{
		cancelCallback?.Invoke();
		Hide();
	}
}
