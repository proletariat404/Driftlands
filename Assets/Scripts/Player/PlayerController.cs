using UnityEngine;
using Pathfinding;

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

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.1f, LayerMask.GetMask("Obstacle"));
        

        if (Input.GetMouseButtonDown(0))
        {
            if (stats.IsStaminaDepleted())
            {
                Debug.Log("体力不足，无法开始移动！");
                return;
            }

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            seeker.StartPath(rb.position, mouseWorldPos, OnPathComplete);
        }
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
        distanceSinceLastStaminaLoss += distanceMoved;

        if (distanceSinceLastStaminaLoss >= 1f)
        {
            int wholeUnits = Mathf.FloorToInt(distanceSinceLastStaminaLoss);
            bool success = stats.ConsumeStamina(wholeUnits);
            if (success)
            {
                distanceSinceLastStaminaLoss -= wholeUnits;
            }
            else
            {
                Debug.Log("体力耗尽，停止移动！");
                path = null;
                return;
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
        // 可以按你的逻辑控制移动输入
        this.enabled = enabled;
        // 或者单独设定一个 allowMove 标志位
    }

}
