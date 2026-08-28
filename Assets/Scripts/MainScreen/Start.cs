using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteButton : MonoBehaviour
{
    // 마우스 커서가 스프라이트 클릭 영역(Collider) 안에서 눌렸을 때 자동 실행
    private void OnMouseDown()
    {
        SceneManager.LoadScene("InGame");
    }
}