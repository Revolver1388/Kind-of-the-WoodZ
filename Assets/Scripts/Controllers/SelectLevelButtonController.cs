// Created by Dylan LeClair 27/03/21
// Last modified 27/03/21 (Dylan LeClair)
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectLevelButtonController : MonoBehaviour
{
    [SerializeField] Button thisButton;
    [SerializeField] int index;

    void Start(){
        if (!GameManager.Instance.GetLevelsPassed()[index])
            thisButton.interactable = false;
    }
    public void OnSceneInt(int i)
    { 
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadSceneAsync(i);
        SceneManager.UnloadSceneAsync(currentScene);

        AudioManager.Instance.CheckMusicTrack();
    }
}
