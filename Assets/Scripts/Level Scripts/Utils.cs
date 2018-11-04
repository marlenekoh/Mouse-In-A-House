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
        yield return new WaitForSeconds(0); // delay 1 second
        SceneManager.LoadScene(levelName);
    }

}
