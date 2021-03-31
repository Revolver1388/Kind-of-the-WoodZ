﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [SerializeField] BoxCollider2D leftBlocker;
    [SerializeField] BoxCollider2D rightBlocker;

    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] GameObject[] enemies;

    [SerializeField] int maxEnemies;
    [SerializeField] int startingEnemies;
    [SerializeField] int bodyCountRequired;
    [SerializeField] float spawnRate;

    private int enemiesDefeated;
    private bool active;
    [SerializeField] Camera _cam;
    private List<GameObject> enemiesAlive = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        leftBlocker.enabled = false;
        rightBlocker.enabled = false;
        _cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            if (enemiesAlive.Count > 0)
            {
                for (int i = 0; i < enemiesAlive.Count; i++)
                {
                    GameObject e = enemiesAlive[i];
                    if (!e.GetComponentInChildren<EnemyBaseClass>().isAlive)
                    {
                        enemiesAlive.Remove(e);
                        enemiesDefeated++;
                    }
                }
            }
        }
    }

    //spawn a random enemy at a random spawn point
    void spawnEnemy()
    {
        GameObject e = Instantiate(enemies[Random.Range(0, enemies.Length)]);
        e.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        e.GetComponentInChildren<SpriteRenderer>().sortingLayerName = "WalkArea";
        enemiesAlive.Add(e);
    }

    //when trigger is crossed by the player enable the blockers and start spawning
    private void OnTriggerEnter2D(Collider2D c)
    {
        if (!active)
        {
            if (c.gameObject.tag == "Player")
            {
                _cam.GetComponent<PlayerCamera>()._holdPos = this.transform;
                _cam.GetComponent<PlayerCamera>().togglePause();
                StartCoroutine(spawnTimer(spawnRate));
                enemiesDefeated = 0;
                active = true;
                leftBlocker.enabled = true;
                rightBlocker.enabled = true;
                if (startingEnemies > 0)
                {
                    for (int i = 0; i > startingEnemies; i++)
                    {
                        spawnEnemy();
                    }
                }
            }
        }
    }

    [SerializeField] GameObject moveArrow;

    //spawn enemies every x seconds equal to your set preference, also destroys object once limits are hit
    IEnumerator spawnTimer (float time)
    {
        do
        {
            if (enemiesAlive.Count < maxEnemies)
            {
                //if enemies defeated + alive is less than bodyCount spawn enemy
                if (enemiesAlive.Count + enemiesDefeated < bodyCountRequired)
                {
                    spawnEnemy();
                }
            }
            yield return new WaitForSeconds(time);
        }
        while (bodyCountRequired > enemiesDefeated);

        moveArrow.SetActive(true);

        yield return new WaitForSeconds(3);

        //unlock camera
        _cam.GetComponent<PlayerCamera>().togglePause();
        _cam.GetComponent<PlayerCamera>()._holdPos = null;
        Destroy(gameObject);
    }
}
