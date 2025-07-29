using UnityEngine;

public class MoveBasedEventTrigger : MonoBehaviour
{
    [Range(0f, 1f)]
    public float triggerProbability = 0.1f;  // �ƶ�ʱ�����¼����ʣ�����0.1��ʾ10%

    private Vector3 lastPosition;
    private float distanceMovedSinceLastCheck = 0f;
    public float checkIntervalDistance = 1f;  // ÿ�ƶ����پ�����һ���¼�����

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // ���㱾֡�ƶ������ۻ�
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        distanceMovedSinceLastCheck += distanceThisFrame;
        lastPosition = transform.position;

        if (distanceMovedSinceLastCheck >= checkIntervalDistance)
        {
            distanceMovedSinceLastCheck = 0f;

            if (IsPlayerMoving())
            {
                TryTriggerEvent();
            }
        }
    }

    private bool IsPlayerMoving()
    {
        // �ж�����Ƿ����ƶ���������ж��ٶȴ���0.01f
        // ��Ҳ���Ի����ж�������������߼�
        return Vector3.Distance(transform.position, lastPosition) > 0.01f;
    }

    private void TryTriggerEvent()
    {
        float rand = Random.Range(0f, 1f);
        if (rand <= triggerProbability)
        {
            TriggerEvent();
        }
    }

    private void TriggerEvent()
    {
        // ����д��Ҫ�������¼������絯����������Ч����������ȵ�
        Debug.Log("�������ƶ��¼���");
        // TODO: ����ʵ���¼����߼�
    }
}
