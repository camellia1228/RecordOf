using UnityEngine;

/// <summary>
/// 세로(vertical) HP바 - 물약통처럼 hp 비율에 따라 Fill 스프라이트를
/// 세로로 스케일(scale)해서 차오르거나 닳는 느낌을 낸다.
/// 이 방식이 동작하려면 Fill 스프라이트의 피벗(Pivot)이 반드시 "아래(Bottom)"로
/// 설정되어 있어야 한다.
///
/// hp가 maxHp보다 작고, 대쉬를 사용하고 있지 않을 때 자동으로 서서히 회복된다.
/// 회복량은 Time.deltaTime을 곱해서 계산하므로 모니터 주사율(프레임률)과 무관하게
/// 항상 "초당 regenRate만큼"의 일정한 속도로 회복된다.
/// </summary>
public class HpBar : MonoBehaviour
{
    [Header("HP Settings")]
    public float hp = 100f;                       // 현재 HP - 외부(Dash.cs 등)에서 직접 접근 가능
    [SerializeField] private float maxHp = 100f;   // 최대 HP (고정값 100)

    [Header("HP Bar Fill Settings")]
    [SerializeField] private Transform fillTransform; // hp에 따라 세로로 늘어나거나 줄어드는 스프라이트 (피벗 = Bottom 필수)

    [Header("HP Regen Settings")]
    [SerializeField] private float regenRate = 5f; // 초당 회복량 (deltaTime과 곱해져서 프레임률 무관하게 적용됨)
    [SerializeField] private Dash dashRef;          // 대쉬 사용 중인지 확인하기 위한 참조 - 인스펙터에서 연결 필요

    private Vector3 originalScale; // 100% 상태일 때의 원래 스케일 값 (기준값)

    void Start()
    {
        if (fillTransform != null)
        {
            originalScale = fillTransform.localScale;
        }
    }

    void Update()
    {
        UpdateBarScale();
        RegenerateHp();
    }

    // 현재 hp 비율만큼 Fill 스프라이트의 y축 스케일을 조절
    private void UpdateBarScale()
    {
        if (fillTransform == null) return;

        hp = Mathf.Clamp(hp, 0f, maxHp); // 버그 수정: hp가 아니라 maxHp를 상한으로 clamp
        float ratio = hp / maxHp; // 0.0 ~ 1.0

        Vector3 scale = fillTransform.localScale;
        scale.y = originalScale.y * ratio; // 세로 길이만 hp 비율만큼 조절 
        scale.x = originalScale.x * ratio;
        fillTransform.localScale = scale;
    }

    // 대쉬를 쓰고 있지 않고 hp가 최대치보다 낮으면 초당 regenRate만큼 서서히 회복
    private void RegenerateHp()
    {
        bool isDashingNow = dashRef != null && dashRef.IsDashing;

        if (!isDashingNow && hp < maxHp)
        {
            // deltaTime을 곱해서 프레임률(주사율)과 무관하게 "초당 일정량"으로 회복되도록 처리
            hp += regenRate * Time.deltaTime;
            hp = Mathf.Clamp(hp, 0f, maxHp);
        }
    }

    /// <summary>데미지 적용 (dash 소모, 피격 등에서 호출)</summary>
    public void TakeDamage(float amount)
    {
        hp -= amount;
        hp = Mathf.Clamp(hp, 0f, maxHp);
    }

    /// <summary>회복</summary>
    public void Heal(float amount)
    {
        hp += amount;
        hp = Mathf.Clamp(hp, 0f, maxHp);
    }

    public bool IsDead()
    {
        return hp <= 0f;
    }
}