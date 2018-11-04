using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour {

    public static AudioClip jump;
    public static AudioClip click;

    public static bool muteSfx;

    static AudioSource audioSrc;
    private static SfxManager instance = null;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        muteSfx = false;
        jump = Resources.Load<AudioClip>("jump");
        click = Resources.Load<AudioClip>("click");
        audioSrc = gameObject.GetComponent<AudioSource>();
        audioSrc.volume = 0.3f;

    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "jump":
                audioSrc.PlayOneShot(jump);
                break;
        }
    }

    public void playClick()
    {
        audioSrc.PlayOneShot(click);
    }

}