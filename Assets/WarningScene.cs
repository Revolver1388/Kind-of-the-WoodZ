using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WarningScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private IEnumerator ChangeScene()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadSceneAsync("Main_Menu");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
