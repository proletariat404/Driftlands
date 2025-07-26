using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果你用 TextMeshPro
using System.Collections;

public class UIHintManager : MonoBehaviour
{
    public static UIHintManager Instance { get; private set; }

    [Header("UI 元素")]
    public CanvasGroup hintCanvasGroup;
    public TextMeshProUGUI hintText; // 或使用 UnityEngine.UI.Text
    public float fadeDuration = 0.2f;

    private Coroutine hintRoutine;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 可选：常驻
        // DontDestroyOnLoad(gameObject);

        // 初始隐藏
        hintCanvasGroup.alpha = 0f;
        hintCanvasGroup.interactable = false;
        hintCanvasGroup.blocksRaycasts = false;
    }

    public void ShowHint(string message, float duration = 2f)
    {
        if (hintRoutine != null)
            StopCoroutine(hintRoutine);

        hintRoutine = StartCoroutine(ShowHintRoutine(message, duration));
    }

    private IEnumerator ShowHintRoutine(string message, float duration)
    {
        hintText.text = message;

        // 淡入
        yield return StartCoroutine(FadeCanvasGroup(hintCanvasGroup, 0f, 1f, fadeDuration));

        // 等待显示时间
        yield return new WaitForSeconds(duration);

        // 淡出
        yield return StartCoroutine(FadeCanvasGroup(hintCanvasGroup, 1f, 0f, fadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, elapsed / time);
            yield return null;
        }
        cg.alpha = to;
    }
}
