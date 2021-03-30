// Created by Dylan LeClair 27/03/21
// Last modified 27/03/21 (Dylan LeClair)
//Last Modified 28/03/21 (Kyle Ennis)
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    [SerializeField] Transform player;
    [SerializeField] bool[] levelsPassed = null;

    int levelProgression = 0;

    void Awake(){
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    void Start(){
        levelProgression = PlayerPrefs.GetInt("LevelProgression");

        for (int i = 0; i < levelsPassed.Length; i++)
            if (PlayerPrefs.GetInt("LevelPassed" + i) != 0)
                levelsPassed[i] = true;
    }

    public void SetLevelPassed(int level){
        levelProgression++;
        levelsPassed[level] = true;

        PlayerPrefs.SetInt("LevelPassed" + level, 1);
        PlayerPrefs.SetInt("LevelProgression", level);
        PlayerPrefs.Save();
    }

    public string GetActiveScene()
    {
        return SceneManager.GetActiveScene().name;
    }
    public Transform GetPlayerTransform()
    {
        return player;
    }

    public int GetLevelProgression(){
        return levelProgression;
    }

    public bool[] GetLevelsPassed(){
        return levelsPassed;
    }
}
