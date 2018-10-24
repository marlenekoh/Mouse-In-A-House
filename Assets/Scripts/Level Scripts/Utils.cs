using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

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
}
