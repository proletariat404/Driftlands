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

            // ���� GameEventUI �� onHide �¼�
            GameEventUI.Instance.onHide += OnUIHide;

            triggered = true;
        }
    }

    private void OnUIHide()
    {
        // ȡ�����ģ������ظ�����
        GameEventUI.Instance.onHide -= OnUIHide;

        // ���ٴ���������
        Destroy(gameObject);

        // ���ֻ��Ҫ���ض��������٣�����ʹ�����´��룺
        // gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        // ȷ������������ʱȡ�����ģ���ֹ�ڴ�й©
        if (GameEventUI.Instance != null)
        {
            GameEventUI.Instance.onHide -= OnUIHide;
        }
    }
}