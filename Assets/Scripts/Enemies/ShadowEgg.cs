using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowEgg : MonoBehaviour
{
    public List<GameObject> enemies = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(Crack());
    }

    IEnumerator Crack()
    {
        yield return new WaitForSeconds(6);

        Instantiate(enemies[Random.Range(0, enemies.Count - 1)], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
