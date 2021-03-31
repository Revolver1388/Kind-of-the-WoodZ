using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IntroScript : MonoBehaviour
{
    Animator _anim;

    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name == "IntroScene") _anim.SetBool("isIntro", true);
        else _anim.SetBool("isIntro", false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
