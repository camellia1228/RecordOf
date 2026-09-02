using UnityEngine;

public class Spin : MonoBehaviour
{
    public float rotationSpeed = 50f; // 회전 속도
    public Transform player;         // 따라갈 플레이어의 Transform
    public Vector3 offset;           // 플레이어와의 거리 유지용 오프셋

    void Start()
    {
        // 플레이어가 할당되지 않았다면 "Player" 태그를 가진 오브젝트를 자동으로 탐색
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        // 플레이어 위치 따라가기
        if (player != null)
        {
            transform.position = player.position + offset;
        }

        // Z축(Vector3.forward)을 기준으로 회전
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}