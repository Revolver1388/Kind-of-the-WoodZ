// Created by Dylan LeClair 27/03/21
// Last modified 20/03/21 (Dylan LeClair)

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Button_Manager : MonoBehaviour {
    [Header("Object References")]
    [SerializeField] AudioMixer audioMixer = null;
    [SerializeField] AudioSource audioSource = null;

    [Header("Panel Switch References")]
    [SerializeField] Button thisButton = null;
    [SerializeField] Button PanelBeingSwitchedToButton = null;
    [SerializeField] GameObject panelToActivate = null;
    [SerializeField] GameObject PanelToDeactivate = null;

    [Header("Interaction Audio")]
    [SerializeField] AudioClip mousedOverAudio = null;
    [SerializeField] AudioClip buttonPressedAudio = null;

    [Header("Scene Traversal")]
    [SerializeField] string goToSceneName = null;

    [Header("Save Settings Variables")]
    [SerializeField] string saveKey = null;
    [SerializeField] string audioMixerChannelKey = null;

    void OnMouseEnter(){
        audioSource.PlayOneShot(mousedOverAudio);
    }

    public void ResetLevelProgression()
    {
        GameManager.Instance.ResetLevelProgression();
    }

    public void OnChangeScene() {
        audioSource.PlayOneShot(buttonPressedAudio);

        int currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadSceneAsync(goToSceneName);
        SceneManager.UnloadSceneAsync(currentScene);

        AudioManager.Instance.CheckMusicTrack();
    }

    public void ContinueGame(){
        audioSource.PlayOneShot(buttonPressedAudio);

        int currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadSceneAsync(GameManager.Instance.GetLevelProgression()+1);
        SceneManager.UnloadSceneAsync(currentScene);

        AudioManager.Instance.CheckMusicTrack();
    }

    public void OnSliderChange(float value) {
        audioMixer.SetFloat(audioMixerChannelKey, Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(saveKey, value);

        PlayerPrefs.Save();
    }

    public void OnToggleChange(bool status) {
        AudioListener.pause = status;
        PlayerPrefs.SetInt(saveKey, (status ? 1 : 0));

        PlayerPrefs.Save();
    }

    public void OnPanelSwicth(){
        thisButton.interactable = false;
        PanelBeingSwitchedToButton.interactable = true;

        PanelToDeactivate.SetActive(false);
        panelToActivate.SetActive(true);
    }

    public void OnQuitGame(){
        Application.Quit();
    }
}
