// Created By Dylan LeClair 27/03/21
// Last modified 27/03/21 (Dylan LeClair)

using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] Text pauseMesssageField;
    [SerializeField] string[] PauseMessageTexts;

    void OnEnable() {
        Time.timeScale = 0;
        pauseMesssageField.text = PauseMessageTexts[Random.Range(0, PauseMessageTexts.Length)];
    }

    public void Resume(){
        gameObject.SetActive(false);
    }

    void OnDisable(){
        Time.timeScale = 1;
    }
}
