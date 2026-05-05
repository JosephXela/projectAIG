using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void NextLevel()
    {
        int nextSceneIdx = SceneManager.GetActiveScene().buildIndex + 1;
        //jika maks scene, tidak ada scene berikutnya
        if (nextSceneIdx >= SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(0); //ke level 1
        }
        else
        {
            SceneManager.LoadScene(nextSceneIdx);
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
