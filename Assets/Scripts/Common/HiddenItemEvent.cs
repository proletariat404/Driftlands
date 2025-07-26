using UnityEngine;
using System.Collections;

public class HiddenItemEvent : MapEventTrigger
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    [Header("发现提示")]
    public string discoverMessage = "你发现了隐藏的宝箱！";

    [Header("显现效果")]
    public float revealDuration = 2f;
    public float flickerSpeed = 0.1f;

    private PlayerController playerController;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false; // 初始隐藏
        if (col != null)
            col.enabled = false;
    }

    protected override void TryTrigger(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null || triggered) return;

        int perception = stats.GetPerception(); // 感知值
        float chance = Random.Range(0f, 100f);

        if (chance < perception)
        {
            triggered = true;
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.SetControlEnabled(false); // 禁止移动

            Debug.Log(discoverMessage); // 可替换为 UI 弹窗

            StartCoroutine(RevealSequence());
        }
    }

    private IEnumerator RevealSequence()
    {
        spriteRenderer.enabled = true;
        float elapsed = 0f;
        bool visible = false;

        // 闪烁动画阶段
        while (elapsed < revealDuration)
        {
            visible = !visible;
            spriteRenderer.color = new Color(1f, 1f, 1f, visible ? 1f : 0.2f);
            elapsed += flickerSpeed;
            yield return new WaitForSeconds(flickerSpeed);
        }

        // 渐变到完全显现
        float fadeTime = 0.5f;
        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            float alpha = Mathf.Lerp(0.2f, 1f, elapsed / fadeTime);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 完全显现
        spriteRenderer.color = Color.white;
        if (col != null)
            col.enabled = true;

        if (playerController != null)
            playerController.SetControlEnabled(true); // 恢复移动
    }
}
