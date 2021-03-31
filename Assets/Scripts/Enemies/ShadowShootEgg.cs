using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowShootEgg : MonoBehaviour
{
    public float movementSpeed;

    Coroutine destroy;

    public void Start()
    {
    }

    private void OnEnable()
    {
        destroy = StartCoroutine(DestroyEgg());
    }


    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + 2 * Mathf.Sign(transform.right.x), transform.position.y), movementSpeed * Time.fixedDeltaTime);

    }

    private void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyEgg()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
    
}
