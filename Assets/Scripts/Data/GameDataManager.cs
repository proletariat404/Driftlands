using UnityEngine;

public class GameDataManager : MonoBehaviour
{
	public static GameDataManager Instance { get; private set; }

	public HiddenHintDatabase HiddenHints { get; private set; }
	public ItemDatabase ItemData { get; private set; }
	public ChestDropDatabase ChestData { get; private set; }

	public RandomEventsWeightDatabase RandomEventsWeightData { get; private set; }
	public RandomEventsDatabase RandomEventsData { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		// 已有数据库加载
		HiddenHints = new HiddenHintDatabase("hidden_hints.json");
		ItemData = new ItemDatabase("item.json");
		ChestData = new ChestDropDatabase("chest.json");

		// 新增随机事件相关数据库加载
		RandomEventsWeightData = new RandomEventsWeightDatabase("random_events_weight.json");
		RandomEventsData = new RandomEventsDatabase("random_events.json");

		DontDestroyOnLoad(gameObject);
	}
}
