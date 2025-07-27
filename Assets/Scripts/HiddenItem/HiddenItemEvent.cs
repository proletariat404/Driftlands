using UnityEngine;
using System.Collections;

public class HiddenItemEvent : MapEventTrigger
{
	private SpriteRenderer spriteRenderer;
	private Collider2D col;

	[Header("提示 ID")]
	public int hintId = 1;

	[Header("显现效果")]
	public float revealDuration = 2f;
	public float flickerSpeed = 0.1f;

	[Header("UI提示持续时间")]
	public float uiHintDuration = 2f;

	private PlayerController playerController;

	protected override void Start()
	{
		base.Start();
		spriteRenderer = GetComponent<SpriteRenderer>();
		col = GetComponent<Collider2D>();
		if (spriteRenderer != null)
			spriteRenderer.enabled = false;
		if (col != null)
			col.enabled = false;
	}

	protected override void TryTrigger(GameObject player)
	{
		PlayerStats stats = player.GetComponent<PlayerStats>();
		if (stats == null || triggered) return;

		int perception = stats.GetPerception();
		float chance = Random.Range(0f, 100f);

		if (chance < perception)
		{
			triggered = true;
			playerController = player.GetComponent<PlayerController>();
			if (playerController != null)
				playerController.SetControlEnabled(false);

			var hint = GameDataManager.Instance.HiddenHints.GetHintById(hintId);
			if (hint != null && UIHintManager.Instance != null)
			{
				UIHintManager.Instance.ShowHint(hint.HintText, uiHintDuration);
			}

			StartCoroutine(RevealSequence());
		}
	}

	private IEnumerator RevealSequence()
	{
		spriteRenderer.enabled = true;
		float elapsed = 0f;
		bool visible = false;

		while (elapsed < revealDuration)
		{
			visible = !visible;
			spriteRenderer.color = new Color(1f, 1f, 1f, visible ? 1f : 0.2f);
			elapsed += flickerSpeed;
			yield return new WaitForSeconds(flickerSpeed);
		}

		float fadeTime = 0.5f;
		elapsed = 0f;
		while (elapsed < fadeTime)
		{
			float alpha = Mathf.Lerp(0.2f, 1f, elapsed / fadeTime);
			spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
			elapsed += Time.deltaTime;
			yield return null;
		}

		spriteRenderer.color = Color.white;
		if (col != null)
			col.enabled = true;

		if (playerController != null)
			playerController.SetControlEnabled(true);
	}
}
