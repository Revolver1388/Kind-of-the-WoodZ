using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggScript : MonoBehaviour
{
    [SerializeField] public bool isHeroEgg;
    [SerializeField] public bool isEnemyEgg;
    [SerializeField] public bool hasHit;
    private Rigidbody2D eggBody;
    // Start is called before the first frame update
    private float startX;
    void Start()
    {
        StartCoroutine(deathTimer());
        isHeroEgg = false;
        isEnemyEgg = false;
        hasHit = false;
        eggBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(((startX - transform.position.x)<-25)||((startX - transform.position.x) > 25))
        {
            eggCollision();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PlayerAttackBox")
        {
            if (collision.gameObject.GetComponentInParent<HeroControls>().facingRight)
            {
                isHeroEgg = true;
                startX = transform.position.x;
                eggBody.velocity = new Vector2(5.0f, 0.0f);
                //start hit animation
            }
            else
            {
                isHeroEgg = true;
                startX = transform.position.x;
                eggBody.velocity = new Vector2(-5.0f, 0.0f);
                //start hit animation
            }
        }
        else if (collision.tag == "EnemyAttackBox")
        {
            if (collision.gameObject.GetComponentInParent<HeroControls>().facingRight)
            {
                isEnemyEgg = true;
                startX = transform.position.x;
                eggBody.velocity = new Vector2(5.0f, 0.0f);
                //start hit animation
            }
            else
            {
                isEnemyEgg = true;
                startX = transform.position.x;
                eggBody.velocity = new Vector2(-5.0f, 0.0f);
                //start hit animation
            }
        }
    }

    public void eggCollision()
    {
        //squish animation
        //Destroy GameObject
        hasHit = true;
        eggBody.velocity = Vector2.zero;
        StartCoroutine(runDown());
    }

    IEnumerator deathTimer()
    {
        yield return new WaitForSeconds(10f);
        if (!isEnemyEgg && !isHeroEgg)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator runDown()
    {
        yield return new WaitForSeconds(0.7f);       
            Destroy(gameObject);
        
    }
}
