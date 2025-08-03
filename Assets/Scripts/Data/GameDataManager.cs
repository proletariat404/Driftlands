using UnityEngine;

public class GameDataManager : MonoBehaviour
{
	public static GameDataManager Instance { get; private set; }

	public HiddenHintDatabase HiddenHints { get; private set; }
	public ItemDatabase ItemData { get; private set; }
	public ChestDropDatabase ChestData { get; private set; }

	public RandomEventsWeightDatabase RandomEventsWeightData { get; private set; }
	public RandomEventsDatabase RandomEventsData { get; private set; }

	public GameEventsDatabase GameEventsData { get; private set; }
	public EventBehaviorDatabase EventBehaviorData { get; private set; } // ��ѡ

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		// ����ͨ�����ñ�
		HiddenHints = new HiddenHintDatabase("hidden_hints.json");
		ItemData = new ItemDatabase("item.json");
		ChestData = new ChestDropDatabase("chest.json");

		// ��������¼�ϵͳ
		RandomEventsWeightData = new RandomEventsWeightDatabase("random_events_weight.json");
		RandomEventsData = new RandomEventsDatabase("random_events.json");

		// ���ؽ����¼�ϵͳ
		GameEventsData = new GameEventsDatabase("game_events.json", "event_behaviors.json");

		// ����ѡ������㻹��Ҫֱ�ӷ���ԭʼ��Ϊ���ݱ�
		EventBehaviorData = new EventBehaviorDatabase("event_behaviors.json");
	}
}
