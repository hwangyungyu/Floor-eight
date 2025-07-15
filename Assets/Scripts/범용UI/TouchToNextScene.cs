using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchToNextScene : MonoBehaviour
{
    public string nextSceneName; // Inspector에서 지정하거나 아래 코드처럼 자동 지정

    void Update()
    {
        // 모바일 터치 또는 마우스 클릭
        if (Input.GetMouseButtonDown(0) || Input.touchCount > 0)
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName); // 이름으로 이동
        }
        else
        {
            // 현재 씬의 build index 기준 다음 씬으로 이동
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}
