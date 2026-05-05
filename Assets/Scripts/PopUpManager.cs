using UnityEngine;

public class PopUpManager : MonoBehaviour
{


    public GameObject gameOverPopup;
    void Start()
    {
        gameOverPopup.SetActive(false);
    }

    public void ShowGameOverPopup()
    {
        gameOverPopup.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideGameOverPopup()
    {
        gameOverPopup.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        
    }
}
