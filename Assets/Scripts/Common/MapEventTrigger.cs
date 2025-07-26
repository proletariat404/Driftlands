using UnityEngine;

public abstract class MapEventTrigger : MonoBehaviour
{
    [Header("��������")]
    public float triggerRadius = 2f;

    protected bool alreadyChecked = false;  // �Ѽ����������ظ�����
    protected bool triggered = false;       // �¼��Ƿ��Ѵ���

    protected virtual void Start()
    {
        // �����ʵ�֣��������� override
    }

    protected virtual void Update()
    {
        if (triggered || alreadyChecked) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= triggerRadius)
        {
            alreadyChecked = true;
            TryTrigger(player);
        }
    }

    /// <summary>
    /// �������ʵ�־��崥���߼�
    /// </summary>
    protected abstract void TryTrigger(GameObject player);
}
