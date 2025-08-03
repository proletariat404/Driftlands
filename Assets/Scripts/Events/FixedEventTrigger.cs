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

			triggered = true;
		}
	}
}

