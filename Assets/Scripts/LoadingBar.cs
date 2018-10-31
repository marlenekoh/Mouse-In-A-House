using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour {
    public Slider slider;
    private AsyncOperation async = null; // When assigned, load is in progress.
    private float startTime;

    private void Start()
    {
        // Level is loading too fast, use artifical loading bar instead
        //StartCoroutine(loadLevel("Level"));'
        InvokeRepeating("incSliderValue", 0, 0.1f);
        slider.value = 0;
    }

    private void FixedUpdate()
    {
        if (slider.value >= 1.0f)
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    private void incSliderValue()
    {
        slider.value = Mathf.Min(slider.value + 0.1f, 1.0f);
    }

    private IEnumerator loadLevel(string levelName)
    {
        async = SceneManager.LoadSceneAsync(levelName);
        yield return async;
    }

}



