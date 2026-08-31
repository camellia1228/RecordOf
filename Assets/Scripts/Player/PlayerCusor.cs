using UnityEngine;

/// <summary>
/// 시스템 기본 마우스 커서를 숨기고 커스텀 스프라이트로 대체.
/// 플레이어 -> 마우스 방향을 계산해서 산나비 스타일의 조준점(Aim Reticle)을 표시한다.
/// Dash.cs, Bullet.cs 등에서 GetAimDirection()으로 조준 방향을 가져다 쓸 수 있음.
/// </summary>
public class PlayerCursor : MonoBehaviour
{
    [Header("Cursor Sprite Settings")]
    [SerializeField] private SpriteRenderer cursorRenderer; // 마우스 위치를 따라다니는 커스텀 커서 스프라이트
    [SerializeField] private Sprite customCursorSprite;      // 교체할 커서 스프라이트

    [Header("Aim Reticle Settings")]
    [SerializeField] private Transform player;          // 조준 기준이 되는 플레이어 Transform
    [SerializeField] private Transform aimReticle;       // 산나비 스타일 조준점 스프라이트
    [SerializeField] private float aimDistance = 1.5f;   // 플레이어로부터 조준점까지의 거리
    [SerializeField] private bool rotateReticle = true;  // 각도에 맞춰 조준점을 회전시킬지 여부

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // 시스템 기본 커서 렌더링 제거
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;

        if (cursorRenderer != null && customCursorSprite != null)
        {
            cursorRenderer.sprite = customCursorSprite;
        }
    }

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        UpdateCursorSprite(mouseWorldPos);
        UpdateAimReticle(mouseWorldPos);
    }

    // 마우스 스크린 좌표 -> 월드 좌표 변환
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(mainCamera.transform.position.z); // 카메라와 z축 평면 거리 보정
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
        worldPos.z = 0f;
        return worldPos;
    }

    // 커스텀 커서 스프라이트를 마우스 위치로 이동
    private void UpdateCursorSprite(Vector3 mouseWorldPos)
    {
        if (cursorRenderer == null) return;
        cursorRenderer.transform.position = mouseWorldPos;
    }

    // 플레이어 -> 마우스 방향을 계산해서 조준점 위치/회전 갱신
    private void UpdateAimReticle(Vector3 mouseWorldPos)
    {
        if (aimReticle == null || player == null) return;

        Vector2 direction = (mouseWorldPos - player.position);
        if (direction.sqrMagnitude < 0.0001f) return;
        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 조준점을 플레이어로부터 일정 거리에 위치시킴 (산나비의 와이어 조준점처럼)
        aimReticle.position = player.position + (Vector3)(direction * aimDistance);

        if (rotateReticle)
        {
            aimReticle.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    /// <summary>
    /// 외부 스크립트(Dash.cs, Bullet.cs 등)에서 현재 조준 방향(정규화된 벡터)을 가져올 때 사용.
    /// </summary>
    public Vector2 GetAimDirection()
    {
        if (player == null || mainCamera == null) return Vector2.right;
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Vector2 dir = (Vector2)(mouseWorldPos - player.position);
        return dir.sqrMagnitude < 0.0001f ? Vector2.right : dir.normalized;
    }
}