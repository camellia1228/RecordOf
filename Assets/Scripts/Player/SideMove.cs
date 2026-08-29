using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class SideMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float acceleration = 10f; // 관성(미끄러짐) 조절

    [Header("Ground Check (Layer Only)")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckHeight = 0.1f; // 바닥 감지 두께

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private float horizontalInput;
    private bool isGrounded;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        // 좌우 입력
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 점프 입력 (바닥에 있을 때만 작동하도록 철저히 제한)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

        Flip();
    }

    void FixedUpdate()
    {
        // 1. 발바닥 위치 계산 (BoxCollider 기준 맨 아래 바닥)
        Vector2 boxCenter = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.min.y - (groundCheckHeight / 2f));
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x * 0.9f, groundCheckHeight);

        // 2. 레이어 기반으로 바닥 감지 (OverlapBox)
        Collider2D hit = Physics2D.OverlapBox(boxCenter, boxSize, 0f, groundLayer);
        isGrounded = (hit != null);

        // 3. 점프 후 상승 중일 때는 바닥 판정을 강제로 꺼서 공중에서 허공 점프(무한 점프)되는 버그 방지
        if (rb.linearVelocity.y > 0.1f)
        {
            isGrounded = false;
        }

        // 4. 관성 이동 적용
        float targetSpeed = horizontalInput * moveSpeed;
        float smoothedSpeed = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(smoothedSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void Flip()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    // 에디터에서 바닥 감지 박스 확인용
    private void OnDrawGizmos()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.color = Color.green;
            Vector2 boxCenter = new Vector2(col.bounds.center.x, col.bounds.min.y - (groundCheckHeight / 2f));
            Vector2 boxSize = new Vector2(col.bounds.size.x * 0.9f, groundCheckHeight);
            Gizmos.DrawWireCube(boxCenter, boxSize);
        }
    }
}