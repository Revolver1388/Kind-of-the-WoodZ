// Created by Dylan Leclair 27/03/21
// Last modified 27/03/21 (Dylan LeClair)

using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] int levelIndex;

    void Start(){
        GameManager.Instance.SetLevelPassed(levelIndex - 1);
    }
}
