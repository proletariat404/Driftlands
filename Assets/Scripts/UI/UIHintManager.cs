using UnityEngine;
using UnityEngine.UI;
using TMPro; // ������� TextMeshPro
using System.Collections;

public class UIHintManager : MonoBehaviour
{
    public static UIHintManager Instance { get; private set; }

    [Header("UI Ԫ��")]
    public CanvasGroup hintCanvasGroup;
    public TextMeshProUGUI hintText; // ��ʹ�� UnityEngine.UI.Text
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

        // ��ѡ����פ
        // DontDestroyOnLoad(gameObject);

        // ��ʼ����
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

        // ����
        yield return StartCoroutine(FadeCanvasGroup(hintCanvasGroup, 0f, 1f, fadeDuration));

        // �ȴ���ʾʱ��
        yield return new WaitForSeconds(duration);

        // ����
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
