using UnityEngine;

public class GameDataManager : MonoBehaviour
{
	public static GameDataManager Instance { get; private set; }

	public HiddenHintDatabase HiddenHints { get; private set; }
	public ItemDatabase ItemData { get; private set; }
	public ChestDropDatabase ChestData { get; private set; }

	void Awake()
	{
		if (Instance != null) { Destroy(gameObject); return; }
		Instance = this;

		HiddenHints = new HiddenHintDatabase("hidden_hints.json");
		ItemData = new ItemDatabase("item.json");
		ChestData = new ChestDropDatabase("chest.json");

	}
}
