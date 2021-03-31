// Created by Dylan LeClair 27/03/21
// Last modified 27/03/21 (Dylan LeClair)
//Last Modified 28/03/21 (Kyle Ennis)
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public List<SpawnZone> spawnZoneList;

    private Transform player;
    [SerializeField] bool[] levelsPassed = null;

    int levelProgression = 1;

    string level1 = "City_Scene";
    string level2 = "Night_Scene";
    string level3 = "Forest_Scene";
    string level4 = "Mountain_Scene";
    string endScene = "EndScene";
    string introScene = "IntroScene";


    void Awake()
    {
        spawnZoneList = new List<SpawnZone>(); // spawn zones add themselves to this list when they are instatiated in scene
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void CheckIfLevelComplete()
    {
        if (spawnZoneList.Count == 0)
        {
            SetLevelPassed(levelProgression);
        }
    }

    void Start(){
        levelProgression = PlayerPrefs.GetInt("LevelProgression");

        for (int i = 0; i < levelsPassed.Length; i++)
            if (PlayerPrefs.GetInt("LevelPassed" + i) != 0)
                levelsPassed[i] = true;
    }

    public void ResetLevelProgression()
    {
        levelProgression = 1;
    }

    public void SetLevelPassed(int level){

        Scene currentScene = SceneManager.GetActiveScene();

        if (level == 4)
        {

            SceneManager.LoadSceneAsync("OutroScene");
            SceneManager.UnloadSceneAsync(currentScene);

            return;
        }

        levelProgression++;
        levelsPassed[level] = true;

        PlayerPrefs.SetInt("LevelPassed" + level, 1);
        PlayerPrefs.SetInt("LevelProgression", level);
        PlayerPrefs.Save();


        SceneManager.LoadSceneAsync(levelProgression);
        SceneManager.UnloadSceneAsync(currentScene);
    }

    public string GetActiveScene()
    {
        return SceneManager.GetActiveScene().name;
    }
    public Transform GetPlayerTransform()
    {
        player = FindObjectOfType<HeroControls>().gameObject.transform;
        return player;
    }

    public int GetLevelProgression(){


        return levelProgression;

    }

    public bool[] GetLevelsPassed(){
        return levelsPassed;
    }
}
