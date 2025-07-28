using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TribeSelectionUI : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private Button[] tribeButtons = new Button[3];
    [SerializeField] private TextMeshProUGUI[] tribeNameTexts = new TextMeshProUGUI[3];  // 显示部族名称
    [SerializeField] private TextMeshProUGUI[] tribeDescriptionTexts = new TextMeshProUGUI[3]; // 显示能力描述
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI selectedTribeText;

    private TribeSystem tribeSystem;
    private TribeType selectedTribeType = TribeType.None;

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

        TribeSystem.OnTribeSelectionComplete += OnTribeSelectionComplete;
    }

    void OnDestroy()
    {
        TribeSystem.OnTribeSelectionComplete -= OnTribeSelectionComplete;
    }

    /// <summary>
    /// 初始化UI（绑定按钮逻辑）
    /// </summary>
    private void InitializeUI()
    {
        for (int i = 0; i < tribeButtons.Length; i++)
        {
            int index = i;

            if (tribeButtons[i] != null)
            {
                tribeButtons[i].onClick.RemoveAllListeners();
                tribeButtons[i].onClick.AddListener(() => OnTribeButtonClicked((TribeType)(index + 1), index));
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
    /// 选择部族按钮点击事件
    /// </summary>
    private void OnTribeButtonClicked(TribeType tribeType, int buttonIndex)
    {
        selectedTribeType = tribeType;
        UpdateButtonStates(buttonIndex);

        // 使用旁边的名称文本来显示已选择部族
        if (selectedTribeText != null && tribeNameTexts[buttonIndex] != null)
            selectedTribeText.text = $"已选择: {tribeNameTexts[buttonIndex].text}";

        if (confirmButton != null)
            confirmButton.interactable = true;
    }

    /// <summary>
    /// 更新按钮高亮状态
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
    /// 确认选择按钮点击
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
    /// 选择完成后隐藏界面
    /// </summary>
    private void OnTribeSelectionComplete()
    {
        HideSelectionPanel();
        Debug.Log("部族选择完成，开始游戏！");
    }

    public void ShowSelectionPanel()
    {
        if (selectionPanel != null)
            selectionPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void HideSelectionPanel()
    {
        if (selectionPanel != null)
            selectionPanel.SetActive(false);

        Time.timeScale = 1f;
    }

    public bool ShouldShowSelection()
    {
        return tribeSystem != null && tribeSystem.HasSelectedTribe() == false;
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
