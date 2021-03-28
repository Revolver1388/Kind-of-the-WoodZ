// Created by Dylan LeClair 27/03/21
// Last modified 27/03/21 (Dylan LeClair)

using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonController : MonoBehaviour
{
    [SerializeField] Button continueButton = null;

    void Start(){
        if (GameManager.Instance.GetLevelProgression() == 0)
            continueButton.interactable = false;
    }
}
