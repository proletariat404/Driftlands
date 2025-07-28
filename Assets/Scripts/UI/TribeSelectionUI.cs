using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TribeSelectionUI : MonoBehaviour
{
    [Header("UI����")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private Button[] tribeButtons = new Button[3];
    [SerializeField] private TextMeshProUGUI[] tribeNameTexts = new TextMeshProUGUI[3];  // ��ʾ��������
    [SerializeField] private TextMeshProUGUI[] tribeDescriptionTexts = new TextMeshProUGUI[3]; // ��ʾ��������
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
    /// ��ʼ��UI���󶨰�ť�߼���
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
    /// ѡ���尴ť����¼�
    /// </summary>
    private void OnTribeButtonClicked(TribeType tribeType, int buttonIndex)
    {
        selectedTribeType = tribeType;
        UpdateButtonStates(buttonIndex);

        // ʹ���Աߵ������ı�����ʾ��ѡ����
        if (selectedTribeText != null && tribeNameTexts[buttonIndex] != null)
            selectedTribeText.text = $"��ѡ��: {tribeNameTexts[buttonIndex].text}";

        if (confirmButton != null)
            confirmButton.interactable = true;
    }

    /// <summary>
    /// ���°�ť����״̬
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
                    colors.normalColor = new Color(1f, 0.9f, 0.5f); // ����ɫ
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
    /// ȷ��ѡ��ť���
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        if (selectedTribeType == TribeType.None)
        {
            Debug.LogWarning("����ѡ��һ�����壡");
            return;
        }

        tribeSystem.SelectTribe(selectedTribeType);
    }

    /// <summary>
    /// ѡ����ɺ����ؽ���
    /// </summary>
    private void OnTribeSelectionComplete()
    {
        HideSelectionPanel();
        Debug.Log("����ѡ����ɣ���ʼ��Ϸ��");
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

    #region ���Է���
    [ContextMenu("ǿ����ʾѡ�����")]
    public void ForceShowSelection()
    {
        ShowSelectionPanel();
    }

    [ContextMenu("����ѡ��״̬")]
    public void ResetSelection()
    {
        selectedTribeType = TribeType.None;

        if (selectedTribeText != null)
            selectedTribeText.text = "��ѡ����";

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
