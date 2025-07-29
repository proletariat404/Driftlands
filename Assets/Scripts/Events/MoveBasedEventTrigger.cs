using UnityEngine;

public class MoveBasedEventTrigger : MonoBehaviour
{
    [Range(0f, 1f)]
    public float triggerProbability = 0.1f;  // 移动时触发事件概率，比如0.1表示10%

    private Vector3 lastPosition;
    private float distanceMovedSinceLastCheck = 0f;
    public float checkIntervalDistance = 1f;  // 每移动多少距离检测一次事件触发

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        // 计算本帧移动距离累积
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
        // 判断玩家是否有移动，这里简单判断速度大于0.01f
        // 你也可以换成判断输入或者其他逻辑
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
        // 这里写你要触发的事件，比如弹窗，播放音效，触发剧情等等
        Debug.Log("触发了移动事件！");
        // TODO: 触发实际事件的逻辑
    }
}
