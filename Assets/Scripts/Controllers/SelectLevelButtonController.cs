// Created by Dylan LeClair 27/03/21
// Last modified 27/03/21 (Dylan LeClair)
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelButtonController : MonoBehaviour
{
    [SerializeField] Button thisButton;
    [SerializeField] int index;

    void Start(){
        if (!GameManager.Instance.GetLevelsPassed()[index])
            thisButton.interactable = false;
    }
}
