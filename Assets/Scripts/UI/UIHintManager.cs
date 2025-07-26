using UnityEngine;
using UnityEngine.UI;
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

	private Coroutine hintRoutine;

	void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

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

	// 新增代码：读取并显示提示文本
	public void LoadHintData(string fileName)
	{
		string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

		if (File.Exists(filePath))
		{
			string jsonContent = File.ReadAllText(filePath);
			HintData[] hintDataArray = JsonUtility.FromJson<HintDataArray>(jsonContent).hints;

			// 假设要显示第一个提示
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
