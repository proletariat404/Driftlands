using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RandomEventUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject eventPanel;
    public TMP_Text eventDescription;
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

        eventDescription.text = description;
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