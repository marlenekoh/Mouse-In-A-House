using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSfxButtonScripts : MonoBehaviour {

    public Button musicOn;
    public Button musicOff;
    public Button sfxOn;
    public Button sfxOff;

    public SfxManager sfxManager;
    public MusicManager musicManager;
    public AudioSource sfxSrc;
    public AudioSource musicSrc;

    // Use this for initialization
    void Start () {
        musicOn.onClick.AddListener(ToggleMusic);
        musicOff.onClick.AddListener(ToggleMusic);

        sfxOn.onClick.AddListener(ToggleSfx);
        sfxOff.onClick.AddListener(ToggleSfx);

        if (MusicManager.muteMusic)
        {
            musicOff.gameObject.SetActive(true);
            musicOn.gameObject.SetActive(false);
        }

        if (MusicManager.muteSfx)
        {
            sfxOff.gameObject.SetActive(true);
            sfxOn.gameObject.SetActive(false);
        }
    }

    public void FixedUpdate()
    {
        sfxManager = GameObject.Find("SfxManager").GetComponent<SfxManager>();
        sfxSrc = sfxManager.getAudioSource();

        musicManager = GameObject.Find("MusicManager").GetComponent<MusicManager>();
        musicSrc = musicManager.getAudioSource();
    }

    public void ToggleMusic()
    {
        MusicManager.muteMusic = !MusicManager.muteMusic;
        if (MusicManager.muteMusic)
        {
            musicSrc.Stop();
            musicSrc.volume = 0f;
            musicSrc.mute = true;
        }
        else
        {
            MusicManager.playBGM();
            musicSrc.volume = 0.3f;
            musicSrc.mute = false;
        }
    }

    public void ToggleSfx()
    {
        MusicManager.muteSfx = !MusicManager.muteSfx;
        if (MusicManager.muteSfx)
        {
            sfxSrc.Stop();
            sfxSrc.mute = true;
            sfxSrc.volume = 0f;
        }
        else
        {
            sfxSrc.mute = false;
            sfxSrc.volume = 0.3f;
        }
    }
}
