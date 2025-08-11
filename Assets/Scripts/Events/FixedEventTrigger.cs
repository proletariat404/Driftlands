using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FixedEventTrigger : MonoBehaviour
{
    public int eventId;
    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (!collision.CompareTag("Player")) return;

        var evt = GameDataManager.Instance.GameEventsData.GetEventById(eventId);
        if (evt != null)
        {
            var behaviorDb = GameDataManager.Instance.EventBehaviorData;
            GameEventUI.Instance.Show(evt, behaviorDb);

            // 订阅 GameEventUI 的 onHide 事件
            GameEventUI.Instance.onHide += OnUIHide;

            triggered = true;
        }
    }

    private void OnUIHide()
    {
        // 取消订阅，避免重复调用
        GameEventUI.Instance.onHide -= OnUIHide;

        // 销毁触发器物体
        Destroy(gameObject);

        // 如果只需要隐藏而不是销毁，可以使用以下代码：
        // gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // 确保在物体销毁时取消订阅，防止内存泄漏
        if (GameEventUI.Instance != null)
        {
            GameEventUI.Instance.onHide -= OnUIHide;
        }
    }
}