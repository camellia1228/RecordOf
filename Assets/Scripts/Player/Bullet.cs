using UnityEngine;

/// <summary>
/// 총알 자체의 동작을 담당한다 (스프라이트 1 = 총알 본체).
/// 중력의 영향을 받으며, ground 레이어와 충돌하면 탄흔(스프라이트 3)을 생성하고 사라진다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float lifeTime = 3f;        // 일정 시간이 지나면 자동 삭제
    [SerializeField] private float gravityScale = 1f;     // 총알에 적용할 중력 스케일
    [SerializeField] private LayerMask groundLayer;       // 탄흔이 생성될 레이어 (ground)

    [Header("Impact Effect")]
    [SerializeField] private GameObject impactDecalPrefab; // 탄흔 프리팹 (스프라이트 3)

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = gravityScale; // 총알이 중력의 영향을 받도록 설정
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// 총알을 지정한 방향/속도로 발사한다. BulletShooter에서 Instantiate 직후 호출.
    /// </summary>
    public void Launch(Vector2 direction, float speed)
    {
        rb.linearVelocity = direction.normalized * speed;

        // 진행 방향으로 총알 스프라이트 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // ground 레이어와 접촉했을 때만 탄흔 생성
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            ContactPoint2D contact = collision.contacts[0];
            SpawnImpactDecal(contact.point, contact.normal);
            Destroy(gameObject);
        }
    }

    // 충돌 지점의 법선 방향에 맞춰 탄흔(스프라이트 3)을 생성
    private void SpawnImpactDecal(Vector2 point, Vector2 normal)
    {
        if (impactDecalPrefab == null) return;

        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg;
        Instantiate(impactDecalPrefab, point, Quaternion.Euler(0f, 0f, angle));
    }
}

/// <summary>
/// 총알 발사를 담당한다. 플레이어(또는 총구) 오브젝트에 부착.
/// 마우스 좌클릭 시 PlayerCursor의 조준 방향으로 총알(스프라이트 1)과
/// 발사 이펙트(스프라이트 2)를 생성한다.
/// </summary>
public class BulletShooter : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField] private GameObject bulletPrefab;       // 총알 프리팹 (스프라이트 1, Bullet 컴포넌트 포함)
    [SerializeField] private GameObject muzzleEffectPrefab; // 발사 이펙트 프리팹 (스프라이트 2)
    [SerializeField] private Transform firePoint;            // 총알이 발사되는 위치(총구)
    [SerializeField] private float bulletSpeed = 15f;
    [SerializeField] private float fireRate = 0.2f;          // 연사 간격(초)
    [SerializeField] private float muzzleEffectLifeTime = 0.15f;

    [Header("References")]
    [SerializeField] private PlayerCursor playerCursor;      // 조준 방향 계산용

    private float lastFireTime;

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= lastFireTime + fireRate)
        {
            Fire();
            lastFireTime = Time.time;
        }
    }

    private void Fire()
    {
        Vector2 direction = playerCursor != null ? playerCursor.GetAimDirection() : Vector2.right;
        if (direction.sqrMagnitude < 0.0001f) direction = Vector2.right;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 총알 생성 및 발사
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.Launch(direction, bulletSpeed);
            }
        }

        // 발사 이펙트 생성 (잠깐 보여주고 제거)
        if (muzzleEffectPrefab != null && firePoint != null)
        {
            GameObject fx = Instantiate(muzzleEffectPrefab, firePoint.position, Quaternion.Euler(0f, 0f, angle));
            Destroy(fx, muzzleEffectLifeTime);
        }
    }
}