using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Utils : MonoBehaviour
{
    public Animator fadeAnim;

    private void Start()
    {
        fadeAnim = GameObject.Find("Fade").GetComponent<Animator>();
        fadeAnim.SetTrigger("fadeIn");
    }

    public void pauseGame(bool pause)
    {
        if (pause)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }

    public void reloadLevel()
    {
        fadeAnim.SetTrigger("fadeOut");
        StartCoroutine(loadLevel("Level"));
    }

    public void quitLevel()
    {
        Application.Quit();
    }

    public void goToMainMenu()
    {
        fadeAnim.SetTrigger("fadeOut");
        StartCoroutine(loadLevel("Main Menu"));
    }

    private IEnumerator loadLevel(string levelName)
    {
        float delay = 0.1f;
        if (levelName == "Main Menu")
        {
            delay = 0.0f;
        }
        yield return new WaitForSeconds(delay); // if delay, cannot load main menu ):
        SceneManager.LoadScene(levelName);
    }

}
