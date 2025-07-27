using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TribeSelectionUI : MonoBehaviour
{
	[Header("UI引用")]
	[SerializeField] private GameObject selectionPanel;
	[SerializeField] private Button[] tribeButtons = new Button[3];
	[SerializeField] private TextMeshProUGUI[] tribeNameTexts = new TextMeshProUGUI[3];
	[SerializeField] private TextMeshProUGUI[] tribeDescriptionTexts = new TextMeshProUGUI[3];
	[SerializeField] private Button confirmButton;
	[SerializeField] private TextMeshProUGUI selectedTribeText;

	private TribeSystem tribeSystem;
	private TribeType selectedTribeType = TribeType.None;
	private TribeData[] availableTribes;

	void Start()
	{
		tribeSystem = FindObjectOfType<TribeSystem>();
		InitializeUI();

		if (!ShouldShowSelection())
		{
			HideSelectionPanel();
		}
		else
		{
			ShowSelectionPanel();
		}

		// 订阅事件
		TribeSystem.OnTribeSelectionComplete += OnTribeSelectionComplete;
	}

	void OnDestroy()
	{
		// 取消订阅事件
		TribeSystem.OnTribeSelectionComplete -= OnTribeSelectionComplete;
	}

	/// <summary>
	/// 初始化UI
	/// </summary>
	private void InitializeUI()
	{
		if (tribeSystem == null)
		{
			Debug.LogError("找不到TribeSystem组件！");
			return;
		}

		availableTribes = tribeSystem.GetAvailableTribes();

		for (int i = 0; i < tribeButtons.Length && i < availableTribes.Length; i++)
		{
			int index = i;
			TribeData tribeData = availableTribes[i];

			if (tribeNameTexts[i] != null)
				tribeNameTexts[i].text = tribeData.tribeName;

			if (tribeDescriptionTexts[i] != null)
				tribeDescriptionTexts[i].text = tribeData.description + "\n" + tribeData.abilityDescription;

			if (tribeButtons[i] != null)
			{
				tribeButtons[i].onClick.RemoveAllListeners();
				tribeButtons[i].onClick.AddListener(() => OnTribeButtonClicked(tribeData.tribeType, index));
			}
		}

		if (confirmButton != null)
		{
			confirmButton.onClick.RemoveAllListeners();
			confirmButton.onClick.AddListener(OnConfirmButtonClicked);
			confirmButton.interactable = false;
		}
	}

	/// <summary>
	/// 部族按钮点击事件
	/// </summary>
	private void OnTribeButtonClicked(TribeType tribeType, int buttonIndex)
	{
		selectedTribeType = tribeType;
		UpdateButtonStates(buttonIndex);

		TribeData selectedTribe = tribeSystem.GetTribeData(tribeType);
		if (selectedTribeText != null && selectedTribe != null)
			selectedTribeText.text = $"已选择: {selectedTribe.tribeName}";

		if (confirmButton != null)
			confirmButton.interactable = true;
	}

	/// <summary>
	/// 更新按钮颜色状态
	/// </summary>
	private void UpdateButtonStates(int selectedIndex)
	{
		for (int i = 0; i < tribeButtons.Length; i++)
		{
			if (tribeButtons[i] != null)
			{
				ColorBlock colors = tribeButtons[i].colors;
				if (i == selectedIndex)
				{
					colors.normalColor = new Color(1f, 0.9f, 0.5f); // 高亮色
					colors.selectedColor = colors.normalColor;
				}
				else
				{
					colors.normalColor = Color.white;
					colors.selectedColor = Color.white;
				}
				tribeButtons[i].colors = colors;
			}
		}
	}

	/// <summary>
	/// 确认按钮点击事件
	/// </summary>
	private void OnConfirmButtonClicked()
	{
		if (selectedTribeType == TribeType.None)
		{
			Debug.LogWarning("请先选择一个部族！");
			return;
		}

		tribeSystem.SelectTribe(selectedTribeType);
	}

	/// <summary>
	/// 部族选择完成事件
	/// </summary>
	private void OnTribeSelectionComplete()
	{
		HideSelectionPanel();
		Debug.Log("部族选择完成，开始游戏！");
	}

	/// <summary>
	/// 显示选择面板并暂停游戏
	/// </summary>
	public void ShowSelectionPanel()
	{
		if (selectionPanel != null)
			selectionPanel.SetActive(true);

		Time.timeScale = 0f;
	}

	/// <summary>
	/// 隐藏选择面板并恢复游戏
	/// </summary>
	public void HideSelectionPanel()
	{
		if (selectionPanel != null)
			selectionPanel.SetActive(false);

		Time.timeScale = 1f;
	}

	/// <summary>
	/// 是否应显示部族选择
	/// </summary>
	public bool ShouldShowSelection()
	{
		return tribeSystem != null && !tribeSystem.HasSelectedTribe();
	}

	#region 调试方法
	[ContextMenu("强制显示选择界面")]
	public void ForceShowSelection()
	{
		ShowSelectionPanel();
	}

	[ContextMenu("重置选择状态")]
	public void ResetSelection()
	{
		selectedTribeType = TribeType.None;

		if (selectedTribeText != null)
			selectedTribeText.text = "请选择部族";

		if (confirmButton != null)
			confirmButton.interactable = false;

		for (int i = 0; i < tribeButtons.Length; i++)
		{
			if (tribeButtons[i] != null)
			{
				ColorBlock colors = tribeButtons[i].colors;
				colors.normalColor = Color.white;
				colors.selectedColor = Color.white;
				tribeButtons[i].colors = colors;
			}
		}
	}
	#endregion
}
