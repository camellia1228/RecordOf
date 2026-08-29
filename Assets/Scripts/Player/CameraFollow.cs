using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // 따라갈 플레이어
    [SerializeField] private float smoothSpeed = 1000f; // 카메라가 따라오는 부드러움 정도

    [Header("Offset Settings")]
    [SerializeField] private float yOffset = 2f; // 플레이어보다 Y축으로 얼마나 더 높게 있을 것인지
    [SerializeField] private float zOffset = -10f; // 2D 카메라는 보통 -10 유지

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산 (플레이어 X 위치, 플레이어 Y + yOffset, 카메라 고정 Z)
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + yOffset, zOffset);

        // 부드럽게 따라오도록 Lerp 적용
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // 카메라 위치 업데이트
        transform.position = smoothedPosition;
    }
} 