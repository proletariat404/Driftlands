using UnityEngine;
using Pathfinding;
using UnityEngine.EventSystems;

[RequireComponent(typeof(PlayerStats))]
public class PlayerController : MonoBehaviour
{
	public float speed = 4f;

	private Seeker seeker;
	private Path path;
	private int currentWaypoint = 0;
	private bool reachedEndOfPath = false;

	private Rigidbody2D rb;
	private PlayerStats stats;

	private float distanceSinceLastStaminaLoss = 0f;

	[Header("移动事件触发配置")]
	[Range(0f, 1f)]
	public float eventTriggerProbability = 0.1f;  // 事件触发概率，0.1 = 10%
	private float distanceSinceLastEventCheck = 0f;
	public float eventCheckInterval = 1f;        // 每走多少米检测一次事件触发

	private RandomEventSystem randomEventSystem;

	public RandomEventUI randomEventUI;  // 用来显示UI的引用

	void Start()
	{
		seeker = GetComponent<Seeker>();
		rb = GetComponent<Rigidbody2D>();
		stats = GetComponent<PlayerStats>();

		// 初始化随机事件系统
		randomEventSystem = new RandomEventSystem(
			GameDataManager.Instance.RandomEventsWeightData,
			GameDataManager.Instance.RandomEventsData
		);

		randomEventSystem.SetBaseTriggerChance(1f);  // 触发概率由PlayerController控制，这里设1
		randomEventSystem.SetLuckValue(stats.GetLuck()); // 传入当前幸运值

		randomEventSystem.OnEventTriggered += OnRandomEventTriggered;
	}

	void Update()
	{
		if (IsPointerOverUI()) return;

#if UNITY_EDITOR || UNITY_STANDALONE
		if (Input.GetMouseButtonDown(0))
		{
			TryStartMovement(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		}
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryStartMovement(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position));
        }
#endif
	}

	private void TryStartMovement(Vector3 targetWorldPos)
	{
		if (stats.IsStaminaDepleted())
		{
			Debug.Log("体力不足，无法开始移动！");
			return;
		}

		targetWorldPos.z = 0;
		seeker.StartPath(rb.position, targetWorldPos, OnPathComplete);
	}

	void FixedUpdate()
	{
		if (path == null || currentWaypoint >= path.vectorPath.Count || stats.IsStaminaDepleted())
			return;

		reachedEndOfPath = false;

		Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
		Vector2 velocity = direction * speed * Time.fixedDeltaTime;
		Vector2 nextPosition = rb.position + velocity;

		float distanceMoved = Vector2.Distance(rb.position, nextPosition);

		// 体力消耗逻辑
		distanceSinceLastStaminaLoss += distanceMoved;
		if (distanceSinceLastStaminaLoss >= 1f)
		{
			int wholeUnits = Mathf.FloorToInt(distanceSinceLastStaminaLoss);
			for (int i = 0; i < wholeUnits; i++)
			{
				bool success = stats.TryConsumeStamina(1f);
				if (!success)
				{
					Debug.Log("体力耗尽，停止移动！");
					path = null;
					return;
				}
			}
			distanceSinceLastStaminaLoss -= wholeUnits;
		}

		// 事件触发判定
		distanceSinceLastEventCheck += distanceMoved;
		if (distanceSinceLastEventCheck >= eventCheckInterval)
		{
			distanceSinceLastEventCheck = 0f;

			if (Random.value <= eventTriggerProbability)
			{
				// 动态更新幸运值，防止变化未同步
				randomEventSystem.SetLuckValue(stats.GetLuck());
				TriggerMoveEvent();
			}
		}

		rb.MovePosition(nextPosition);

		float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
		if (distanceToWaypoint < 0.1f)
		{
			currentWaypoint++;
		}
	}

	public void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			path = p;
			currentWaypoint = 0;
		}
	}

	public void SetControlEnabled(bool enabled)
	{
		this.enabled = enabled;
	}

	private bool IsPointerOverUI()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }
        return false;
#else
        return false;
#endif
	}

	// 触发随机事件系统
	private void TriggerMoveEvent()
	{
		randomEventSystem.TryTriggerEvent();
	}

	// 处理随机事件触发结果
	private void OnRandomEventTriggered(RandomEventsDatabase.RandomEventData evt)
	{
		if (evt != null)
		{
			Debug.Log($"触发事件：{evt.event_text}");
			// 调用UI显示
			randomEventUI?.ShowEvent(evt.event_text);

			// TODO: 这里还可以处理奖励逻辑
		}
		else
		{
			Debug.Log("未触发具体随机事件");
		}
	}
}
