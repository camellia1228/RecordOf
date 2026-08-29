using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동 전용 네임스페이스

[RequireComponent(typeof(Collider2D))] // 클릭 감지를 위해 2D 콜라이더가 필요합니다.
public class StartButton : MonoBehaviour
{
    [Header("Load Scene Name")]
    [SerializeField] private string targetSceneName;

    // 스프라이트를 클릭했을 때 실행
    private void OnMouseUpAsButton()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError("이동할 씬 이름이 설정되지 않았습니다!");
        }
    }
}