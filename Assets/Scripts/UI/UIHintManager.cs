using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;

public class UIHintManager : MonoBehaviour
{
	public static UIHintManager Instance { get; private set; }

	[Header("UI Ԫ��")]
	public CanvasGroup hintCanvasGroup;
	public TextMeshProUGUI hintText;
	public float fadeDuration = 0.2f;

	[Header("��ʾĬ�ϳ���ʱ�䣨�룩")]
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

	// ����ʱ����ʱ�䣬ʹ��Ĭ�ϳ���ʱ��
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
		// ��������
		if (GameInputManager.Instance != null)
			GameInputManager.Instance.DisableInput();

		hintText.text = message;

		// ����
		yield return StartCoroutine(FadeCanvasGroup(hintCanvasGroup, 0f, 1f, fadeDuration));

		// �ȴ���ʾʱ��
		yield return new WaitForSeconds(duration);

		// ����
		yield return StartCoroutine(FadeCanvasGroup(hintCanvasGroup, 1f, 0f, fadeDuration));

		// ��������
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

	// ��ȡ����ʾ��ʾ�ı�ʾ������ɾ��
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
