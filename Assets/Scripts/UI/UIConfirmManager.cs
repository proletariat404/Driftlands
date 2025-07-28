using TMPro;
using UnityEngine;
using System;

public class UIConfirmManager : MonoBehaviour
{
	public static UIConfirmManager Instance { get; private set; }

	[Header("UI ‘™Àÿ")]
	public CanvasGroup confirmCanvasGroup;
	public TextMeshProUGUI messageText;
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
		messageText.text = message;
		confirmCallback = onConfirm;
		cancelCallback = onCancel;

		confirmCanvasGroup.alpha = 1f;
		confirmCanvasGroup.blocksRaycasts = true;
		confirmCanvasGroup.interactable = true;

		// ‘›Õ£ ‰»Î
		GameInputManager.Instance?.DisableInput();
	}

	public void Hide()
	{
		confirmCanvasGroup.alpha = 0f;
		confirmCanvasGroup.blocksRaycasts = false;
		confirmCanvasGroup.interactable = false;

		// ª÷∏¥ ‰»Î
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
