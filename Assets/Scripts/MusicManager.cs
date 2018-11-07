using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static AudioClip BGM1;
    public static AudioClip BGM2;

    public static bool muteMusic;
    public static bool muteSfx;

    static AudioSource audioSrc;
    private static MusicManager instance = null;

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
        muteMusic = false;
        muteSfx = false;

        BGM1 = Resources.Load<AudioClip>("BGM1");

        audioSrc = gameObject.GetComponent<AudioSource>();
        audioSrc.volume = 0.3f;
    }

    private void FixedUpdate()
    {
        playBGM();
    }

    public static void PauseBGM()
    {
        audioSrc.Pause();
    }

    public static void playBGM()
    {
        if (!muteMusic && !audioSrc.isPlaying)
        {
            audioSrc.PlayOneShot(BGM1);
        }
    }

    public AudioSource getAudioSource()
    {
        return audioSrc;
    }

}