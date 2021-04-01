using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMicController : MonoBehaviour
{
    public GameObject OffMicButton;
    public GameObject OnMicButton;

    public void OnMic_Click()
    {
        OnMicButton.SetActive(false);
        OffMicButton.SetActive(true);
        PlayerPrefs.SetInt("MicMode", 1);
        Debug.Log("Micmode is 1");
    }

    public void OffMic_Click()
    {
        OnMicButton.SetActive(true);
        OffMicButton.SetActive(false);
        PlayerPrefs.SetInt("MicMode", 0);

        Debug.Log("Micmode is 0");


    }

}
