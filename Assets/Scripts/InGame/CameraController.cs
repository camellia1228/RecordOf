using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("추적 대상")]
    public Transform target; // 따라갈 플레이어의 Transform

    [Header("카메라 설정")]
    public float smoothSpeed = 5f; // 카메라 추적 속도 (높을수록 빠르게 따라감)
    public Vector3 offset = new Vector3(0f, 0f, -10f); // 2D 카메라는 Z축 -10 유지가 필수!

    void LateUpdate()
    {
        if (target == null) return;

        // 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 현재 위치에서 목표 위치로 부드럽게 이동 (Lerp 사용)
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 카메라 위치 업데이트
        transform.position = smoothedPosition;
    }
}