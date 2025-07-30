using UnityEngine;
using TMPro;
using UnityEngine.UI; // 添加这个命名空间
using System.Collections;

public class UIHintManager : MonoBehaviour
{
	public static UIHintManager Instance { get; private set; }

	[Header("UI 元素")]
	public CanvasGroup hintCanvasGroup;
	public Text hintText; // 改为 Text 类型
	public TextMeshProUGUI tmpHintText; // 新增 TextMeshProUGUI 引用
	public float fadeDuration = 0.2f;

	[Header("提示默认持续时间（秒）")]
	public float defaultHintDuration = 2f;

	private Coroutine hintRoutine;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		hintCanvasGroup.alpha = 0f;
		hintCanvasGroup.interactable = false;
		hintCanvasGroup.blocksRaycasts = false;
	}

	public void ShowHint(string message)
	{
		ShowHint(message, defaultHintDuration);
	}

	public void ShowHint(string message, float duration)
	{
		if (hintRoutine != null)
			StopCoroutine(hintRoutine);

		hintRoutine = StartCoroutine(ShowHintRoutine(message, duration));
	}

	private IEnumerator ShowHintRoutine(string message, float duration)
	{
		// 禁用输入
		if (GameInputManager.Instance != null)
			GameInputManager.Instance.DisableInput();

		// 设置文本
		if (tmpHintText != null)
			tmpHintText.text = message;
		else if (hintText != null)
			hintText.text = message;

		// 淡入
		yield return StartCoroutine(FadeCanvasGroup(hintCanvasGroup, 0f, 1f, fadeDuration));

		// 等待显示时间
		yield return new WaitForSeconds(duration);

		// 淡出
		yield return StartCoroutine(FadeCanvasGroup(hintCanvasGroup, 1f, 0f, fadeDuration));

		// 启用输入
		if (GameInputManager.Instance != null)
			GameInputManager.Instance.EnableInput();
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
