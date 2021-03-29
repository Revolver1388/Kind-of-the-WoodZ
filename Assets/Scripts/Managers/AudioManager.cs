using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer = null;

    readonly string[] mixerChannel = { "Master", "Music", "SFX", "Menu" };
    readonly string[] mixerKey = { "MasterVolume", "MusicVolume", "SFXVolume", "MenuVolume" };

    void Start(){
        if (PlayerPrefs.GetInt("Mute") != 0)
            AudioListener.pause = true;

        for (int i = 0; i < mixerKey.Length; i++)
        {
            audioMixer.SetFloat(mixerChannel[i], Mathf.Log10(PlayerPrefs.GetFloat(mixerKey[i])) * 20);
        }
    }
}
