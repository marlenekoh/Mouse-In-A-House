using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour {

    public static AudioClip jump;
    public static AudioClip click;

    public static bool muteSfx;

    static AudioSource audioSrc;

    // Use this for initialization
    void Start()
    {
        muteSfx = false;
        jump = Resources.Load<AudioClip>("jump");
        click = Resources.Load<AudioClip>("click");
        audioSrc = GetComponent<AudioSource>();

    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case ("jump"):
                if (!audioSrc.isPlaying)
                    audioSrc.PlayOneShot(jump);
                break;
        }
    }

    public void playClick()
    {
        audioSrc.PlayOneShot(click);
    }

}