using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxManager : MonoBehaviour {

    public static AudioClip jump;
    public static AudioClip click;
    public static AudioClip catDeathCry;
    public static AudioClip killCat;
    public static AudioClip gameOverSound;
    public static AudioClip catSpawn;
    public static AudioClip chargingCat;
    public static AudioClip jumpingCat;
    public static AudioClip goodJob;

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
        jump = Resources.Load<AudioClip>("loudJump");
        click = Resources.Load<AudioClip>("ButtonClick");
        catDeathCry = Resources.Load<AudioClip>("Cat Death");
        killCat = Resources.Load<AudioClip>("KillCatSound");
        gameOverSound = Resources.Load<AudioClip>("Game Over");
        catSpawn = Resources.Load<AudioClip>("Cat Spawn");
        chargingCat = Resources.Load<AudioClip>("Charging Cat");
        jumpingCat = Resources.Load<AudioClip>("softJump");
        goodJob = Resources.Load<AudioClip>("Good Job");

        audioSrc = gameObject.GetComponent<AudioSource>();
        audioSrc.volume = 0.3f;

    }

    public static void PlaySound(string clip)
    {
        if (!muteSfx)
        {
            switch (clip)
            {
                case "jump":
                    audioSrc.PlayOneShot(jump);
                    break;
                case "catDeathCry":
                    audioSrc.PlayOneShot(catDeathCry);
                    break;
                case "killCat":
                    audioSrc.PlayOneShot(killCat);
                    break;
                case "gameOverSound":
                    audioSrc.PlayOneShot(gameOverSound);
                    break;
                case "catSpawn":
                    audioSrc.PlayOneShot(catSpawn);
                    break;
                case "chargingCat":
                    audioSrc.PlayOneShot(chargingCat);
                    break;
                case "jumpingCat":
                    audioSrc.PlayOneShot(jumpingCat);
                    break;
                case "goodJob":
                    audioSrc.PlayOneShot(goodJob);
                    break;
            }
        }
    }

    public void playClick()
    {
        if (!muteSfx)
        {
            audioSrc.PlayOneShot(click);
        }
    }

    public AudioSource getAudioSource()
    {
        return audioSrc;
    }

}