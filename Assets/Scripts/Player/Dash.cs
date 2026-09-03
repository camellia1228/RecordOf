using UnityEngine;

/// <summary>
/// 마우스 우클릭을 누르는 "그 순간"의 커서 방향으로 스프라이트를 회전시키고
/// 그 방향으로 고정된 채 느린 일정 속도로 직선 이동한다 (홀드 방식 대쉬).
/// 홀드 중에는 방향이 절대 바뀌지 않으며, 손을 떼면 이동이 멈추고 회전이 원래대로 돌아온다.
/// 대쉬를 누르고 있는 동안 0.1초마다 hp -= 1, hp가 0 이하가 되면 더 이상 대쉬를 사용할 수 없다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 1f;         // 홀드 중 이동 속도 (천천히 이동하도록 기본값 낮게)
    [SerializeField] private float hpDrainInterval = 0.1f;  // hp가 깎이는 주기 (초)
    [SerializeField] private float hpDrainAmount = 1f;      // 주기마다 깎이는 hp량

    [Header("References")]
    [SerializeField] private PlayerCursor playerCursor; // 커서 방향 계산용 - 반드시 인스펙터에서 연결할 것
    [SerializeField] private HpBar hpBar;                // hp 관리용
    [SerializeField] private Transform spriteTransform;  // 회전시킬 스프라이트 Transform (비워두면 자기 자신)

    private Rigidbody2D rb;
    private bool isDashing = false;
    private bool dashLocked = false; // hp 소진으로 대쉬가 영구적으로 막혔는지 여부
    private Quaternion originalRotation;
    private float originalGravity;
    private float hpDrainTimer = 0f;   // 0.1초 주기를 세기 위한 누적 타이머
    private Vector2 dashDirection;     // 대쉬 시작 순간에 고정되는 방향 (홀드 중 변하지 않음)

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
        // 우클릭을 누르는 순간 -> 이 시점의 커서 방향으로 고정해서 대쉬 시작
        if (Input.GetMouseButtonDown(1) && !isDashing && !dashLocked)
        {
            StartDash();
        }

        // 우클릭을 누르고 있는 동안 -> 고정된 방향으로 계속 이동 + 0.1초마다 hp 소모
        // (방향은 다시 계산하지 않음. StartDash에서 정한 dashDirection 그대로 유지)
        if (isDashing && Input.GetMouseButton(1) && !dashLocked)
        {
            ApplyDashMovement();
            DrainHpOverTime();
        }

        // 손을 떼거나 hp 소진으로 잠긴 순간 -> 대쉬 종료
        if (isDashing && (Input.GetMouseButtonUp(1) || dashLocked))
        {
            EndDash();
        }
    }

    // 대쉬 시작: 이 순간의 커서 방향을 고정하고, 그 방향으로 한 번만 회전 적용
    private void StartDash()
    {
        isDashing = true;
        hpDrainTimer = 0f;

        // 방향을 여기서 딱 한 번만 계산해서 저장 -> 홀드 중 절대 다시 계산하지 않음
        Vector2 dir = playerCursor != null ? playerCursor.GetAimDirection() : Vector2.right;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        dashDirection = dir;

        // 고정된 방향으로 스프라이트 회전 (한 번만 적용, 이후 유지)
        originalRotation = spriteTransform.rotation;
        float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;
        spriteTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 이동 중 중력 영향 제거
        originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        ApplyDashMovement();
    }

    // 고정된 dashDirection으로 매 프레임 속도만 재적용 (방향 재계산 없음 -> 직선 이동)
    private void ApplyDashMovement()
    {
        rb.linearVelocity = dashDirection * dashSpeed;
    }

    // hpDrainInterval(0.1초)마다 hpDrainAmount(1)만큼 hp를 깎음
    private void DrainHpOverTime()
    {
        if (hpBar == null) return;

        hpDrainTimer += Time.deltaTime;
        if (hpDrainTimer >= hpDrainInterval)
        {
            hpDrainTimer -= hpDrainInterval; // 정확한 주기 유지를 위해 초기화 대신 빼기
            hpBar.TakeDamage(hpDrainAmount);

            // 깎인 직후 hp가 0 이하가 됐으면 다음 프레임까지 기다리지 않고 바로 잠금
            if (hpBar.IsDead())
            {
                dashLocked = true;
            }
        }
    }

    // 대쉬 종료: 이동/중력/회전 원상복구
    private void EndDash()
    {
        isDashing = false;

        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;

        spriteTransform.rotation = originalRotation;
    }

    /// <summary>현재 대쉬를 사용할 수 있는 상태인지 확인 (UI 표시 등에 활용 가능)</summary>
    public bool CanDash()
    {
        return !dashLocked && !isDashing;
    }
}