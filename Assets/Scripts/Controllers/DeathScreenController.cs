// Created by Dylan LeClair 27/03/21
// Last modified 27/03/21 (Dylan Leclair)

using UnityEngine;
using UnityEngine.UI;

public class DeathScreenController : MonoBehaviour
{
    [Header("Text Field References")]
    [SerializeField] Text deathNoteField;
    [SerializeField] Text adviceField;

    [SerializeField] string[] deathNoteText;
    [SerializeField] string[] adviceText;

    void Start(){
        deathNoteField.text = deathNoteText[Random.Range(0, deathNoteText.Length)];
        adviceField.text = adviceText[Random.Range(0, adviceText.Length)];
    }
}
