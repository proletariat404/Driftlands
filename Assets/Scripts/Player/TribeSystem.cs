using UnityEngine;
using System;

/// <summary>
/// 部族类型枚举
/// </summary>
public enum TribeType
{
	None,     // 未选择
	HuangDi,  // 黄帝部族
	YanDi,    // 炎帝部族
	ChiYou    // 蚩尤部族
}

/// <summary>
/// 部族数据结构
/// </summary>
[Serializable]
public class TribeData
{
	public TribeType tribeType;
	public string tribeName;
	public string description;
	public string abilityDescription;

	public TribeData(TribeType type, string name, string desc, string ability)
	{
		tribeType = type;
		tribeName = name;
		description = desc;
		abilityDescription = ability;
	}
}

public class TribeSystem : MonoBehaviour
{
	// 单例实例
	public static TribeSystem Instance { get; private set; }

	// 部族选择事件
	public static event Action<TribeType> OnTribeSelected;
	public static event Action OnTribeSelectionComplete;

	[Header("系统设置")]
	[SerializeField] private bool autoDestroyAfterSelection = true; // 选择完成后是否销毁

	private TribeData[] availableTribes;
	private PlayerStats playerStats;

	void Awake()
	{
		// 单例模式
		if (Instance == null)
		{
			Instance = this;
			InitializeTribes();
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	void Start()
	{
		// 查找PlayerStats组件
		FindPlayerStats();
	}

	/// <summary>
	/// 查找PlayerStats组件
	/// </summary>
	private void FindPlayerStats()
	{
		if (playerStats == null)
		{
			playerStats = FindObjectOfType<PlayerStats>();
			if (playerStats == null)
			{
				Debug.LogWarning("未找到PlayerStats组件，将在选择部族时重新查找");
			}
		}
	}

	/// <summary>
	/// 初始化部族数据
	/// </summary>
	private void InitializeTribes()
	{
		availableTribes = new TribeData[]
		{
			new TribeData(
				TribeType.HuangDi,
				"黄帝部族",
				"体力+5",
				"移动时有15%概率不消耗体力"
			),
			new TribeData(
				TribeType.YanDi,
				"炎帝部族",
				"感知+5",
				"采集时有20%概率获得额外奖励"
			),
			new TribeData(
				TribeType.ChiYou,
				"蚩尤部族",
				"灵性+5",
				"遇到异兽时可以与异兽交流"
			)
		};
	}

	#region 部族选择相关
	/// <summary>
	/// 获取所有可选部族
	/// </summary>
	public TribeData[] GetAvailableTribes()
	{
		return availableTribes;
	}

	/// <summary>
	/// 选择部族
	/// </summary>
	public void SelectTribe(TribeType tribeType)
	{
		// 确保PlayerStats存在
		if (playerStats == null)
		{
			FindPlayerStats();
			if (playerStats == null)
			{
				Debug.LogError("无法找到PlayerStats组件，无法应用部族选择！");
				return;
			}
		}

		// 检查是否已经选择过
		if (playerStats.HasSelectedTribe())
		{
			Debug.LogWarning("部族已经选择过了！");
			return;
		}

		// 应用部族选择到PlayerStats
		playerStats.ApplyTribeSelection(tribeType);

		// 触发事件
		OnTribeSelected?.Invoke(tribeType);
		OnTribeSelectionComplete?.Invoke();

		Debug.Log($"选择了部族: {GetTribeData(tribeType).tribeName}");

		// 选择完成后可以选择销毁此组件
		if (autoDestroyAfterSelection)
		{
			// 延迟销毁，确保事件处理完成
			Invoke(nameof(DestroyTribeSystem), 0.1f);
		}
	}

	/// <summary>
	/// 根据部族类型获取部族数据
	/// </summary>
	public TribeData GetTribeData(TribeType tribeType)
	{
		foreach (var tribe in availableTribes)
		{
			if (tribe.tribeType == tribeType)
				return tribe;
		}
		return null;
	}

	/// <summary>
	/// 获取特殊能力描述（为UI提供支持）
	/// </summary>
	public string GetSpecialAbilityDescription(TribeType tribeType)
	{
		TribeData tribeData = GetTribeData(tribeType);
		return tribeData != null ? tribeData.abilityDescription : "";
	}

	/// <summary>
	/// 是否已选择部族
	/// </summary>
	public bool HasSelectedTribe()
	{
		return playerStats != null && playerStats.HasSelectedTribe();
	}
	#endregion

	/// <summary>
	/// 销毁部族系统
	/// </summary>
	private void DestroyTribeSystem()
	{
		Instance = null;
		Destroy(gameObject);
		Debug.Log("部族选择完成，TribeSystem已销毁");
	}

	#region 调试和测试方法
	/// <summary>
	/// 重置部族选择（仅用于测试）
	/// </summary>
	[ContextMenu("重置部族选择")]
	public void ResetTribeSelection()
	{
		if (playerStats != null)
		{
			playerStats.ResetTribeSelection();
		}
		Debug.Log("部族选择已重置");
	}

	/// <summary>
	/// 显示所有部族信息
	/// </summary>
	[ContextMenu("显示部族信息")]
	public void ShowAllTribesInfo()
	{
		Debug.Log("=== 可选部族 ===");
		foreach (var tribe in availableTribes)
		{
			Debug.Log($"{tribe.tribeName}: {tribe.description} | {tribe.abilityDescription}");
		}
	}
	#endregion
}