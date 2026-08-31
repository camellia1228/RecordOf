using System.Collections;
using UnityEngine;

/// <summary>
/// 마우스 우클릭 시 커서 방향으로 스프라이트를 회전시키고 대쉬한다.
/// 대쉬가 끝나면 회전을 원래대로 되돌린다.
/// 대쉬 1회당 hp -= 5, 대쉬 후 hp가 0 이하가 되면 더 이상 대쉬를 사용할 수 없다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Dash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashHpCost = 5f;

    [Header("References")]
    [SerializeField] private PlayerCursor playerCursor; // 커서 방향 계산용
    [SerializeField] private HpBar hpBar;                // hp 관리용
    [SerializeField] private Transform spriteTransform;  // 회전시킬 스프라이트 Transform (비워두면 자기 자신)

    private Rigidbody2D rb;
    private bool isDashing = false;
    private bool dashLocked = false; // hp 소진으로 대쉬가 영구적으로 막혔는지 여부
    private Quaternion originalRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteTransform == null) spriteTransform = transform;
        originalRotation = spriteTransform.rotation;
    }

    void Update()
    {
        // 우클릭, 대쉬 중이 아니고, hp 소진으로 잠기지 않았을 때만 실행
        if (Input.GetMouseButtonDown(1) && !isDashing && !dashLocked)
        {
            StartCoroutine(DoDash());
        }
    }

    private IEnumerator DoDash()
    {
        isDashing = true;

        // 커서 방향 계산
        Vector2 dir = playerCursor != null ? playerCursor.GetAimDirection() : Vector2.right;
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;

        // 대쉬 방향으로 스프라이트 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        originalRotation = spriteTransform.rotation;
        spriteTransform.rotation = Quaternion.Euler(0f, 0f, angle);

        // hp 소모
        if (hpBar != null)
        {
            hpBar.TakeDamage(dashHpCost);
        }

        // 대쉬 중에는 중력 영향을 잠시 끄고 목표 방향으로 밀어줌
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = dir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = originalGravity;
        rb.linearVelocity = Vector2.zero;

        // 회전을 원래대로 복구
        spriteTransform.rotation = originalRotation;

        isDashing = false;

        // 대쉬를 사용한 결과 hp가 0 이하라면 더 이상 대쉬를 사용할 수 없도록 잠금
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