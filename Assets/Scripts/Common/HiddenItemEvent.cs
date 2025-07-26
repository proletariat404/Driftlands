using UnityEngine;
using System.Collections;

public class HiddenItemEvent : MapEventTrigger
{
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    [Header("������ʾ")]
    public string discoverMessage = "�㷢�������صı��䣡";

    [Header("����Ч��")]
    public float revealDuration = 2f;
    public float flickerSpeed = 0.1f;

    private PlayerController playerController;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false; // ��ʼ����
        if (col != null)
            col.enabled = false;
    }

    protected override void TryTrigger(GameObject player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null || triggered) return;

        int perception = stats.GetPerception(); // ��ֵ֪
        float chance = Random.Range(0f, 100f);

        if (chance < perception)
        {
            triggered = true;
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
                playerController.SetControlEnabled(false); // ��ֹ�ƶ�

            Debug.Log(discoverMessage); // ���滻Ϊ UI ����

            StartCoroutine(RevealSequence());
        }
    }

    private IEnumerator RevealSequence()
    {
        spriteRenderer.enabled = true;
        float elapsed = 0f;
        bool visible = false;

        // ��˸�����׶�
        while (elapsed < revealDuration)
        {
            visible = !visible;
            spriteRenderer.color = new Color(1f, 1f, 1f, visible ? 1f : 0.2f);
            elapsed += flickerSpeed;
            yield return new WaitForSeconds(flickerSpeed);
        }

        // ���䵽��ȫ����
        float fadeTime = 0.5f;
        elapsed = 0f;
        while (elapsed < fadeTime)
        {
            float alpha = Mathf.Lerp(0.2f, 1f, elapsed / fadeTime);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ��ȫ����
        spriteRenderer.color = Color.white;
        if (col != null)
            col.enabled = true;

        if (playerController != null)
            playerController.SetControlEnabled(true); // �ָ��ƶ�
    }
}
