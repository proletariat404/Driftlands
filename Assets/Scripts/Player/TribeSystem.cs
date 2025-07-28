using UnityEngine;
using System;

/// <summary>
/// 部族类型
/// </summary>
public enum TribeType
{
    None,
    HuangDi,
    YanDi,
    ChiYou
}

/// <summary>
/// 部族系统：负责处理选择逻辑，不负责显示文案
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
                Debug.LogWarning("未找到 PlayerStats，将在选择时重试");
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
                Debug.LogError("无法找到 PlayerStats，选择失败");
                return;
            }
        }

        if (playerStats.HasSelectedTribe())
        {
            Debug.LogWarning("已经选择过部族！");
            return;
        }

        playerStats.ApplyTribeSelection(tribeType);
        OnTribeSelected?.Invoke(tribeType);
        OnTribeSelectionComplete?.Invoke();

        Debug.Log($"部族选择完成：{tribeType}");

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
        Debug.Log("TribeSystem 已销毁");
    }

    #region 调试方法
    [ContextMenu("重置部族选择")]
    public void ResetTribeSelection()
    {
        if (playerStats != null)
        {
            playerStats.ResetTribeSelection();
        }
        Debug.Log("已重置部族选择");
    }
    #endregion
}
