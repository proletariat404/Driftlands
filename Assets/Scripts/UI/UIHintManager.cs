using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;

public class UIHintManager : MonoBehaviour
{
	public static UIHintManager Instance { get; private set; }

	[Header("UI 元素")]
	public CanvasGroup hintCanvasGroup;
	public TextMeshProUGUI hintText;
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

	// 调用时不传时间，使用默认持续时间
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

	// 读取并显示提示文本示例（可删）
	public void LoadHintData(string fileName)
	{
		string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

		if (File.Exists(filePath))
		{
			string jsonContent = File.ReadAllText(filePath);
			HintData[] hintDataArray = JsonUtility.FromJson<HintDataArray>(jsonContent).hints;

			if (hintDataArray.Length > 0)
			{
				ShowHint(hintDataArray[0].HintText, hintDataArray[0].Duration);
			}
		}
		else
		{
			Debug.LogError($"File not found: {filePath}");
		}
	}

	[System.Serializable]
	public class HintDataArray
	{
		public HintData[] hints;
	}

	[System.Serializable]
	public class HintData
	{
		public int ID;
		public string HintText;
		public float Duration;
		public int Type;
		public string Column4;
	}
}
