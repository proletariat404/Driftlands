using UnityEngine;

public abstract class MapEventTrigger : MonoBehaviour
{
    [Header("触发设置")]
    public float triggerRadius = 2f;

    protected bool alreadyChecked = false;  // 已检测过，避免重复触发
    protected bool triggered = false;       // 事件是否已触发

    protected virtual void Start()
    {
        // 父类空实现，方便子类 override
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
    /// 子类必须实现具体触发逻辑
    /// </summary>
    protected abstract void TryTrigger(GameObject player);
}
