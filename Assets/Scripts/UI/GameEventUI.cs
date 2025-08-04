using System;
using UnityEngine;
using UnityEngine.UI;

public class GameEventUI : MonoBehaviour
{
    public static GameEventUI Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject panel;
    public Text titleText;
    public Text descriptionText;
    public Text resultText;               // 结果文本UI，记得场景关联
    public Transform buttonContainer;
    public GameObject optionButtonPrefab;

    public Action<EventBehaviorDatabase.BehaviorData> onBehaviorSelected;

    private PlayerStats playerStats;
    private GameEventBehaviorHandler behaviorHandler;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        panel.SetActive(false);

        playerStats = FindObjectOfType<PlayerStats>();

        behaviorHandler = FindObjectOfType<GameEventBehaviorHandler>();
        if (behaviorHandler == null)
        {
            GameObject handlerObj = new GameObject("GameEventBehaviorHandler");
            behaviorHandler = handlerObj.AddComponent<GameEventBehaviorHandler>();
        }
    }

    public void Show(GameEventsDatabase.GameEventData data, EventBehaviorDatabase eventBehaviorDb)
    {
        panel.SetActive(true);
        ClearButtons();

        titleText.text = data.title;
        descriptionText.text = data.description;
        descriptionText.gameObject.SetActive(true);
        resultText.gameObject.SetActive(false);
        buttonContainer.gameObject.SetActive(true);

        var behaviors = eventBehaviorDb.GetBehaviorsByEventId(data.event_id);
        foreach (var behavior in behaviors)
        {
            if (!ConditionChecker.IsConditionMet(behavior.condition))
                continue;

            GameObject btnObj = Instantiate(optionButtonPrefab, buttonContainer);
            var btn = btnObj.GetComponent<Button>();
            var txt = btnObj.GetComponentInChildren<Text>();

            float spiritualityBonus = playerStats != null ? playerStats.GetSpirituality() * 0.01f : 0f;
            float finalRate = Mathf.Clamp01(behavior.rate + spiritualityBonus);

            if (behavior.display_text == "战斗")
            {
                txt.text = behavior.display_text;
            }
            else
            {
                string rateStr = FormatRateWithColor(finalRate);
                txt.text = $"{behavior.display_text} ({rateStr})";
            }

            var behaviorCopy = behavior;
            btn.onClick.AddListener(() =>
            {
                // 先隐藏UI按钮和描述文本，准备显示结果
                HideButtonsAndDescription();

                if (behaviorCopy.display_text == "战斗")
                {
                    // 战斗选项直接关闭面板，不显示结果
                    behaviorHandler.ExecuteBehavior(behaviorCopy, 0f);
                    Hide();
                }
                else
                {
                    // 传递计算好的成功率执行行为
                    behaviorHandler.ExecuteBehavior(behaviorCopy, finalRate);
                }

                onBehaviorSelected?.Invoke(behaviorCopy);
            });
        }
    }

    private string FormatRateWithColor(float rate)
    {
        string color;
        if (rate >= 0.7f) color = "green";
        else if (rate >= 0.4f) color = "yellow";
        else color = "red";

        return $"<color={color}>{(rate * 100f):F0}%</color>";
    }

    public void ShowResultText(string result)
    {
        if (resultText == null)
        {
            Debug.LogWarning("未设置 resultText UI 文本组件");
            return;
        }
        resultText.text = result;
        resultText.gameObject.SetActive(true);
    }

    private void HideButtonsAndDescription()
    {
        buttonContainer.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
    }

    public void Hide()
    {
        panel.SetActive(false);
        ClearButtons();
        onBehaviorSelected = null;

        if (resultText != null)
            resultText.gameObject.SetActive(false);

        if (descriptionText != null)
            descriptionText.gameObject.SetActive(true);

        if (buttonContainer != null)
            buttonContainer.gameObject.SetActive(true);
    }

    private void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
