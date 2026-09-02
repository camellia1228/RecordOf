using UnityEngine;

/// <summary>
/// 마우스 우클릭을 누르고 있는 동안 커서 방향으로 스프라이트를 회전시키고
/// 느린 일정 속도로 계속 이동한다 (홀드 방식 대쉬).
/// 손을 떼면 이동이 멈추고 회전이 원래대로 돌아온다.
/// 대쉬를 새로 시작할 때마다(누르는 순간) hp -= 5, hp가 0 이하가 되면 더 이상 대쉬를 사용할 수 없다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 4f;      // 홀드 중 이동 속도 (천천히 이동하도록 기본값 낮게)
    [SerializeField] private float dashHpCost = 5f;       // 대쉬를 새로 시작할 때 소모되는 hp

    [Header("References")]
    [SerializeField] private PlayerCursor playerCursor; // 커서 방향 계산용 - 반드시 인스펙터에서 연결할 것
    [SerializeField] private HpBar hpBar;                // hp 관리용
    [SerializeField] private Transform spriteTransform;  // 회전시킬 스프라이트 Transform (비워두면 자기 자신)

    private Rigidbody2D rb;
    private bool isDashing = false;
    private bool dashLocked = false; // hp 소진으로 대쉬가 영구적으로 막혔는지 여부
    private Quaternion originalRotation;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteTransform == null) spriteTransform = transform;
        originalRotation = spriteTransform.rotation;

        // playerCursor가 연결 안 됐으면 미리 경고 (방향이 항상 오른쪽으로 고정되는 원인)
        if (playerCursor == null)
        {
            Debug.LogWarning("[Dash] Player Cursor 참조가 비어있습니다. 인스펙터에서 연결해주세요.");
        }
    }

    void Update()
    {
        // 우클릭을 누르는 순간 -> 대쉬 시작
        if (Input.GetMouseButtonDown(1) && !isDashing && !dashLocked)
        {
            StartDash();
        }

        // 우클릭을 누르고 있는 동안 -> 매 프레임 방향 갱신하며 계속 이동
        if (isDashing && Input.GetMouseButton(1) && !dashLocked)
        {
            UpdateDashDirection();
        }

        // 손을 떼거나 hp 소진으로 잠긴 순간 -> 대쉬 종료
        if (isDashing && (Input.GetMouseButtonUp(1) || dashLocked))
        {
            EndDash();
        }
    }

    // 대쉬 시작: hp 소모, 중력 끄기
    private void StartDash()
    {
        isDashing = true;
        originalRotation = spriteTransform.rotation;
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // hp 소모는 대쉬를 시작하는 순간 1회만 적용
        if (hpBar != null)
        {
            hpBar.TakeDamage(dashHpCost);
        }

        UpdateDashDirection();
    }

    // 홀드 중 매 프레임 커서 방향으로 회전 + 이동
    private void UpdateDashDirection()
    {
        Vector2 dir = playerCursor != null ? playerCursor.GetAimDirection() : Vector2.right;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        spriteTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        rb.linearVelocity = dir * dashSpeed;
    }

    // 대쉬 종료: 이동/중력/회전 원상복구
    private void EndDash()
    {
        isDashing = false;

        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;

        spriteTransform.rotation = originalRotation;

        // 이번 대쉬로 인해 hp가 0 이하가 됐으면 이후 대쉬 완전히 잠금
        if (hpBar != null && hpBar.IsDead())
        {
            dashLocked = true;
        }
    }

    /// <summary>현재 대쉬를 사용할 수 있는 상태인지 확인 (UI 표시 등에 활용 가능)</summary>
    public bool CanDash()
    {
        return !dashLocked && !isDashing;
    }
}