using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControls : MonoBehaviour
{
    //Stats
    [SerializeField] private int playerHealth = 100;
    [SerializeField] private int playerEnergy = 100;
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private bool canTakeDamage = true;
    private bool canCharge = true;
    public float attackSpeedDelay = 0.5f;
    
    //Movement Variables
    public float horizontalSpeed = 8f;
    public float verticalSpeed = 4f;
    private Rigidbody2D rigidBod;
    [SerializeField] bool canMove = true;
    private bool facingRight = true;
    [Range(0, 1.0f)]
    [SerializeField] float movementSmooth = 0.1f;
    private Vector3 velocity = Vector3.zero;
    private float horizontalMove;
    private float verticalMove;

    //Attack Variables
    private CircleCollider2D attackZone;
    private int attackDamage = 10;


    void Awake()
    {
        rigidBod = GetComponent<Rigidbody2D>();
        attackZone = GetComponent<CircleCollider2D>();
        attackZone.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                //start or continue charging animation/sprite
                energyCharge();
            }
            else
            {
                horizontalMove = Input.GetAxis("Horizontal");
                verticalMove = Input.GetAxis("Vertical");
                moveCharacter(horizontalMove, verticalMove);
                if (Input.GetKeyDown(KeyCode.J))
                {
                    attack();
                }
            }
        }
    }

    //Movement script via Rigidbody 2D velocity
    public void moveCharacter(float hMove, float vMove)
    {
        if(canMove)
        {
            Vector3 targetV = new Vector2(hMove * horizontalSpeed, vMove * verticalSpeed);

            rigidBod.velocity = Vector3.SmoothDamp(rigidBod.velocity, targetV, ref velocity, movementSmooth);


            if(hMove > 0 && !facingRight)
            {
                flip();
            }
            else if(hMove < 0 && facingRight)
            {
                flip();
            }
        }
    }

    //Simple flip method for facing left/right
    private void flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);
    }

    //Attack script that also calls the delay numerator to avoid attack spam
    private void attack()
    {
        if (canAttack)
        {
            attackZone.enabled = true;
            canAttack = false;
            StartCoroutine(attackDelay());
            Debug.Log("Attack");
            if (playerEnergy > 0)
            {
                playerEnergy -= 5;
            }
            else
            {
                playerEnergy = 0;
            }
        }
    }

    //Energy Charge while holding space bar, to add - voice intensity detection to increase charge
    private void energyCharge()
    {
        if(canCharge)
        {
            canCharge = false;
            StartCoroutine(energyDelay());

            //Get Value from Microphone for charge float of 1-100 or 0-1
            int energyScream = 5; //*value

            if (playerEnergy < 100)
            {
                playerEnergy += energyScream;
            }
            else
            {
                playerEnergy = 100;
            }
        }
    }

    //Take Damage from enemy method
    public void hitOurHero(int damage)
    {
        if (canTakeDamage)
        {
            canTakeDamage = false;
            playerHealth -= damage;
            StartCoroutine(damageDelay());
            if (playerHealth <= 0)
            {
                isAlive = false;
                //dead animation/sprite
            }
            else
            {
                //minor flashing animation?
            }
        }
    }

    //Return attack damage for enemies hit by attack
    public int getDamage()
    {
        return attackDamage;
    }

    IEnumerator attackDelay()
    {
        yield return new WaitForSeconds(attackSpeedDelay);
        attackZone.enabled = false;
        canAttack = true;
    }

    IEnumerator energyDelay()
    {
        yield return new WaitForSeconds(0.7f);
        canCharge = true;
    }

    IEnumerator damageDelay()
    {
        yield return new WaitForSeconds(1.0f);
        canTakeDamage = true;
        //stop minor flashing animation?
    }
}
