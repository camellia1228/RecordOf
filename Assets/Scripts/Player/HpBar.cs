using UnityEngine;

/// <summary>
/// 플레이어 HP를 관리하고, hp 수치에 따라 HP바 스프라이트의 y좌표를 조절한다.
/// 최대 HP는 100으로 고정.
/// </summary>
public class HpBar : MonoBehaviour
{
    [Header("HP Settings")]
    public float hp = 100f;                       // 현재 HP - 외부(Dash.cs 등)에서 직접 접근 가능
    [SerializeField] private float maxHp = 100f;   // 최대 HP (고정값 100)

    [Header("HP Bar Sprite Settings")]
    [SerializeField] private Transform barTransform; // y좌표를 움직일 HP바 스프라이트의 Transform
    [SerializeField] private float minY = -1f;        // hp가 0일 때의 y좌표
    [SerializeField] private float maxY = 1f;         // hp가 maxHp(100)일 때의 y좌표

    void Update()
    {
        UpdateBarPosition();
    }

    // 현재 hp 비율에 따라 스프라이트 y좌표를 갱신
    private void UpdateBarPosition()
    {
        if (barTransform == null) return;

        hp = Mathf.Clamp(hp, 0f, maxHp);
        float ratio = hp / maxHp;

        Vector3 pos = barTransform.localPosition;
        pos.y = Mathf.Lerp(minY, maxY, ratio);
        barTransform.localPosition = pos;
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