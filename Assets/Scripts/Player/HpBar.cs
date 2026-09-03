using UnityEngine;

/// <summary>
/// 세로(vertical) HP바 - 물약통처럼 hp 비율에 따라 Fill 스프라이트를
/// 세로로 스케일(scale)해서 차오르거나 닳는 느낌을 낸다.
/// 이 방식이 동작하려면 Fill 스프라이트의 피벗(Pivot)이 반드시 "아래(Bottom)"로
/// 설정되어 있어야 한다. (설정 방법은 아래 설명 참고)
/// </summary>
public class HpBar : MonoBehaviour
{
    [Header("HP Settings")]
    public float hp = 100f;                       // 현재 HP - 외부(Dash.cs 등)에서 직접 접근 가능
    [SerializeField] private float maxHp = 100f;   // 최대 HP (고정값 100)

    [Header("HP Bar Fill Settings")]
    [SerializeField] private Transform fillTransform; // hp에 따라 세로로 늘어나거나 줄어드는 스프라이트 (피벗 = Bottom 필수)

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
    }

    // 현재 hp 비율만큼 Fill 스프라이트의 y축 스케일을 조절
    private void UpdateBarScale()
    {
        if (fillTransform == null) return;

        hp = Mathf.Clamp(hp, 0f, maxHp);
        float ratio = hp / maxHp; // 0.0 ~ 1.0

        Vector3 scale = fillTransform.localScale;
        scale.y = originalScale.y * ratio; // 세로 길이만 hp 비율만큼 조절 
        fillTransform.localScale = scale;
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