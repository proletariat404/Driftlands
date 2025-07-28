using UnityEngine;
using System;

/// <summary>
/// ��������
/// </summary>
public enum TribeType
{
    None,
    HuangDi,
    YanDi,
    ChiYou
}

/// <summary>
/// ����ϵͳ��������ѡ���߼�����������ʾ�İ�
/// </summary>
public class TribeSystem : MonoBehaviour
{
    public static TribeSystem Instance { get; private set; }

    public static event Action<TribeType> OnTribeSelected;
    public static event Action OnTribeSelectionComplete;

    [SerializeField] private bool autoDestroyAfterSelection = true;

    private PlayerStats playerStats;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        FindPlayerStats();
    }

    private void FindPlayerStats()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null)
            {
                Debug.LogWarning("δ�ҵ� PlayerStats������ѡ��ʱ����");
            }
        }
    }

    public void SelectTribe(TribeType tribeType)
    {
        if (playerStats == null)
        {
            FindPlayerStats();
            if (playerStats == null)
            {
                Debug.LogError("�޷��ҵ� PlayerStats��ѡ��ʧ��");
                return;
            }
        }

        if (playerStats.HasSelectedTribe())
        {
            Debug.LogWarning("�Ѿ�ѡ������壡");
            return;
        }

        playerStats.ApplyTribeSelection(tribeType);
        OnTribeSelected?.Invoke(tribeType);
        OnTribeSelectionComplete?.Invoke();

        Debug.Log($"����ѡ����ɣ�{tribeType}");

        if (autoDestroyAfterSelection)
        {
            Invoke(nameof(DestroyTribeSystem), 0.1f);
        }
    }

    public bool HasSelectedTribe()
    {
        return playerStats != null && playerStats.HasSelectedTribe();
    }

    private void DestroyTribeSystem()
    {
        Instance = null;
        Destroy(gameObject);
        Debug.Log("TribeSystem ������");
    }

    #region ���Է���
    [ContextMenu("���ò���ѡ��")]
    public void ResetTribeSelection()
    {
        if (playerStats != null)
        {
            playerStats.ResetTribeSelection();
        }
        Debug.Log("�����ò���ѡ��");
    }
    #endregion
}
