using UnityEngine;

public class Spin : MonoBehaviour
{
public float rotationSpeed = 50f; // 회전 속도

void Update()
{
    // Z축(Vector3.forward)을 기준으로 회전
    transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
}
}