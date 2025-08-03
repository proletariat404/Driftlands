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
	public EventBehaviorDatabase EventBehaviorData { get; private set; } // 可选

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);

		// 加载通用配置表
		HiddenHints = new HiddenHintDatabase("hidden_hints.json");
		ItemData = new ItemDatabase("item.json");
		ChestData = new ChestDropDatabase("chest.json");

		// 加载随机事件系统
		RandomEventsWeightData = new RandomEventsWeightDatabase("random_events_weight.json");
		RandomEventsData = new RandomEventsDatabase("random_events.json");

		// 加载交互事件系统
		GameEventsData = new GameEventsDatabase("game_events.json", "event_behaviors.json");

		// （可选）如果你还需要直接访问原始行为数据表
		EventBehaviorData = new EventBehaviorDatabase("event_behaviors.json");
	}
}
