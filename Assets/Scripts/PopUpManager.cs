using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public GameObject gameOverPopup;
    void Start()
    {
        gameOverPopup.SetActive(false); //sembunyikan popup saat game mulai
    }

    public void ShowGameOverPopup()
    {
        gameOverPopup.SetActive(true); //tampilkan popup game over di layar
        Time.timeScale = 0f; //pause seluruh game
    }

    public void HideGameOverPopup()
    {
        gameOverPopup.SetActive(false); //sembunyikan popup
        Time.timeScale = 1f; //resume game 
    }
}
