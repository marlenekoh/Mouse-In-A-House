using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour
{

    public void pauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    public void reloadLevel()
    {
        SceneManager.LoadScene("Level");
    }

    public void quitLevel()
    {
        Application.Quit();
    }
}
