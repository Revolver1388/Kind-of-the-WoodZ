using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;
using UnityEngine.SceneManagement;

public class IntroTextHelper : MonoBehaviour
{
    [SerializeField] GameObject textObject;
    Febucci.UI.TextAnimatorPlayer textAnimatorPlayer;

    private string city = "Level 1\nEscape from Grid City" ;

    private string mountain = "Level 4\nKing o' the Mountain";

    private string forest = "Level 2\nOff the Grid and into into da Woodz";

    private string night = "Level 3\nThe Long Dark Nightime of the CurdleJam";

    private string currentScene;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;

        textAnimatorPlayer = gameObject.GetComponent<Febucci.UI.TextAnimatorPlayer>();
        textObject.SetActive(true);

        switch (currentScene)
        {
            case "City_Scene":
                StartCoroutine(startHelper(city));
                break;
            case "Night_Scene":
                StartCoroutine(startHelper(night));
                break;
            case "Mountain_Scene":
                StartCoroutine(startHelper(mountain));
                break;
            case "Forest_Scene":
                StartCoroutine(startHelper(forest));
                break;
        }

    }

    private IEnumerator startHelper(string sceneName)
    {
       
        textAnimatorPlayer.ShowText(sceneName);
        yield return new WaitUntil(() => textAnimatorPlayer.textAnimator.allLettersShown);
        yield return new WaitForSeconds(3);
        textObject.SetActive(false);
    }
}
