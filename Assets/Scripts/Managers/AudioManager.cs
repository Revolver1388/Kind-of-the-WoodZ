//Modified 28/03/21 (Kyle Ennis) ****Basic setup

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioMixer audioMixer = null;

    readonly string[] mixerChannel = { "Master", "Music", "SFX", "Menu" };
    readonly string[] mixerKey = { "MasterVolume", "MusicVolume", "SFXVolume", "MenuVolume" };

    #region AudioSources
    [SerializeField] AudioSource levelMusic;
    [SerializeField] AudioSource sfxSource;
    #endregion
    #region Audio Clips
    public SfxClip[] sfx;
    public LevelMusic[] lvl_Music;
    #endregion

    readonly float levelMusicDelay = 0.4f;

    void Awake()
    {
        Instance = this;
    }

    void Start(){
        for (int i = 0; i < mixerChannel.Length; i++)
        {
            audioMixer.SetFloat(mixerKey[i], PlayerPrefs.GetFloat(mixerChannel[i]));
        }
            
        StartCoroutine(PlayMusic("Menu"));
    }

    public void CheckMusicTrack()
    {
        Invoke(nameof(StartNewMusic), 0.5f);
    }

    void StartNewMusic()
    {
        for (int i = 2; i < 5; i++)
        {
            if (SceneManager.GetActiveScene().buildIndex == i)
            {
                StopMusic();
                StartCoroutine(PlayMusic("Level"));
                return;
            }
        }

        StopMusic();
        StartCoroutine(PlayMusic("Menu"));
    }

    public IEnumerator PlayMusic(string clipName)
    {
        StopMusic();
        yield return new WaitForSeconds(levelMusicDelay);
        foreach (LevelMusic song in lvl_Music) { if (song.name == clipName) levelMusic.clip = song.clip; levelMusic.Play(); }
        levelMusic.Play();
        levelMusic.loop = true;
    }

    public void PlayOneShotByName(string sound)
    { foreach (SfxClip clip in sfx) if (clip.name == sound) sfxSource.PlayOneShot(clip.clip); }

    void FadeMusic()
    {
        //   levelMusic.
    }

    public void StopMusic()
    {
        levelMusic.Stop();
    }


    [System.Serializable]
    public struct LevelMusic { public string name; public AudioClip clip; }

    [System.Serializable]
    public struct SfxClip { public string name; public AudioClip clip; }

}
