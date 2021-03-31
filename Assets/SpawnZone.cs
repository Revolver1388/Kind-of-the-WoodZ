using System.Collections;
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

    private List<GameObject> enemiesAlive = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        leftBlocker.enabled = false;
        rightBlocker.enabled = false;
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //lock camera
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

    //spawn enemies every x seconds equal to your set preference, also destroys object once limits are hit
    IEnumerator spawnTimer (float time)
    {
        do
        {
            if (enemiesAlive.Count < maxEnemies)
            {
                spawnEnemy();
            }
            yield return new WaitForSeconds(time);
        }
        while (bodyCountRequired > enemiesDefeated);

        //unlock camera
        Destroy(gameObject);
    }
}
