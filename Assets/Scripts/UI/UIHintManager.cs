using UnityEngine;
using TMPro;
using UnityEngine.UI; // �����������ռ�
using System.Collections;

public class UIHintManager : MonoBehaviour
{
	public static UIHintManager Instance { get; private set; }

	[Header("UI Ԫ��")]
	public CanvasGroup hintCanvasGroup;
	public Text hintText; // ��Ϊ Text ����
	public TextMeshProUGUI tmpHintText; // ���� TextMeshProUGUI ����
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

		// �����ı�
		if (tmpHintText != null)
			tmpHintText.text = message;
		else if (hintText != null)
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
}
