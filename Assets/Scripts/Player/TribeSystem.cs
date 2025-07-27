using UnityEngine;
using System;

/// <summary>
/// ��������ö��
/// </summary>
public enum TribeType
{
	None,     // δѡ��
	HuangDi,  // �Ƶ۲���
	YanDi,    // �׵۲���
	ChiYou    // ��Ȳ���
}

/// <summary>
/// �������ݽṹ
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
	// ����ʵ��
	public static TribeSystem Instance { get; private set; }

	// ����ѡ���¼�
	public static event Action<TribeType> OnTribeSelected;
	public static event Action OnTribeSelectionComplete;

	[Header("ϵͳ����")]
	[SerializeField] private bool autoDestroyAfterSelection = true; // ѡ����ɺ��Ƿ�����

	private TribeData[] availableTribes;
	private PlayerStats playerStats;

	void Awake()
	{
		// ����ģʽ
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
		// ����PlayerStats���
		FindPlayerStats();
	}

	/// <summary>
	/// ����PlayerStats���
	/// </summary>
	private void FindPlayerStats()
	{
		if (playerStats == null)
		{
			playerStats = FindObjectOfType<PlayerStats>();
			if (playerStats == null)
			{
				Debug.LogWarning("δ�ҵ�PlayerStats���������ѡ����ʱ���²���");
			}
		}
	}

	/// <summary>
	/// ��ʼ����������
	/// </summary>
	private void InitializeTribes()
	{
		availableTribes = new TribeData[]
		{
			new TribeData(
				TribeType.HuangDi,
				"�Ƶ۲���",
				"����+5",
				"�ƶ�ʱ��15%���ʲ���������"
			),
			new TribeData(
				TribeType.YanDi,
				"�׵۲���",
				"��֪+5",
				"�ɼ�ʱ��20%���ʻ�ö��⽱��"
			),
			new TribeData(
				TribeType.ChiYou,
				"��Ȳ���",
				"����+5",
				"��������ʱ���������޽���"
			)
		};
	}

	#region ����ѡ�����
	/// <summary>
	/// ��ȡ���п�ѡ����
	/// </summary>
	public TribeData[] GetAvailableTribes()
	{
		return availableTribes;
	}

	/// <summary>
	/// ѡ����
	/// </summary>
	public void SelectTribe(TribeType tribeType)
	{
		// ȷ��PlayerStats����
		if (playerStats == null)
		{
			FindPlayerStats();
			if (playerStats == null)
			{
				Debug.LogError("�޷��ҵ�PlayerStats������޷�Ӧ�ò���ѡ��");
				return;
			}
		}

		// ����Ƿ��Ѿ�ѡ���
		if (playerStats.HasSelectedTribe())
		{
			Debug.LogWarning("�����Ѿ�ѡ����ˣ�");
			return;
		}

		// Ӧ�ò���ѡ��PlayerStats
		playerStats.ApplyTribeSelection(tribeType);

		// �����¼�
		OnTribeSelected?.Invoke(tribeType);
		OnTribeSelectionComplete?.Invoke();

		Debug.Log($"ѡ���˲���: {GetTribeData(tribeType).tribeName}");

		// ѡ����ɺ����ѡ�����ٴ����
		if (autoDestroyAfterSelection)
		{
			// �ӳ����٣�ȷ���¼��������
			Invoke(nameof(DestroyTribeSystem), 0.1f);
		}
	}

	/// <summary>
	/// ���ݲ������ͻ�ȡ��������
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
	/// ��ȡ��������������ΪUI�ṩ֧�֣�
	/// </summary>
	public string GetSpecialAbilityDescription(TribeType tribeType)
	{
		TribeData tribeData = GetTribeData(tribeType);
		return tribeData != null ? tribeData.abilityDescription : "";
	}

	/// <summary>
	/// �Ƿ���ѡ����
	/// </summary>
	public bool HasSelectedTribe()
	{
		return playerStats != null && playerStats.HasSelectedTribe();
	}
	#endregion

	/// <summary>
	/// ���ٲ���ϵͳ
	/// </summary>
	private void DestroyTribeSystem()
	{
		Instance = null;
		Destroy(gameObject);
		Debug.Log("����ѡ����ɣ�TribeSystem������");
	}

	#region ���ԺͲ��Է���
	/// <summary>
	/// ���ò���ѡ�񣨽����ڲ��ԣ�
	/// </summary>
	[ContextMenu("���ò���ѡ��")]
	public void ResetTribeSelection()
	{
		if (playerStats != null)
		{
			playerStats.ResetTribeSelection();
		}
		Debug.Log("����ѡ��������");
	}

	/// <summary>
	/// ��ʾ���в�����Ϣ
	/// </summary>
	[ContextMenu("��ʾ������Ϣ")]
	public void ShowAllTribesInfo()
	{
		Debug.Log("=== ��ѡ���� ===");
		foreach (var tribe in availableTribes)
		{
			Debug.Log($"{tribe.tribeName}: {tribe.description} | {tribe.abilityDescription}");
		}
	}
	#endregion
}