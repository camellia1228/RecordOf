using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필수

public class Start : MonoBehaviour
{
    public void ChangeToInGame()
    {
        SceneManager.LoadScene("InGame"); 
    }
}