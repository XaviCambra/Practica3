using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public bool GameIsOver = false;


    public void Setup()
    {
        gameObject.SetActive(true);
    }

    public void RestartButton()
    {
        gameObject.SetActive(false);
        GameController.GetGameController().GetPlayer().RestartGame();
        GameController.GetGameController().GetPlayer().Revive();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
