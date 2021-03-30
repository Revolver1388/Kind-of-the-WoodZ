using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControls : MonoBehaviour
{
    //Stats
    [SerializeField] private int playerHealth = 100;
    [SerializeField] public int playerEnergy = 50;
    [SerializeField] private bool isAlive = true;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private bool canTakeDamage = true;
    private bool canCharge = true;

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
    [SerializeField] bool canJump;

    //Attack Variables
    [SerializeField] CircleCollider2D attackZone;
    [SerializeField] int attackDamage;

    //Charging Variables
    [SerializeField] private int energyCharged = 0;
    private bool startCharging = false;
    [SerializeField] GameObject egg;

    //Animator
    Animator _anim;
    [SerializeField] GameObject chargeUp;


    //Microphone
    private AudioMeasure audioMeasure;

    void Awake()
    {
        audioMeasure = FindObjectOfType<AudioMeasure>();       
        rigidBod = GetComponentInParent<Rigidbody2D>();
        attackZone = GetComponentInChildren<CircleCollider2D>();
        attackZone.enabled = false;
        _anim = GetComponent<Animator>();
        attackDamage = 0;
        canJump = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (isAlive)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                rigidBod.velocity = Vector2.zero;
                //start or continue charging animation/sprite
                //energyCharge();
                horizontalMove = 0;
                verticalMove = 0;
                moveCharacter(horizontalMove, verticalMove);

                _anim.SetBool("isCharge", true);
                chargeUp.SetActive(true);
                chargeUp.GetComponent<Animator>().SetBool("isCharge", true);
            }
            else
            {
                horizontalMove = Input.GetAxis("Horizontal");
                verticalMove = Input.GetAxis("Vertical");
                moveCharacter(horizontalMove, verticalMove);
                chargeUp.SetActive(false);

                //Charging Resets
                if (startCharging)
                {
                    _anim.SetBool("isCharge", false);
                    energyCharged = 0;
                    startCharging = false;
                    chargeUp.GetComponent<Animator>().SetBool("isCharge", false);
                }


                //Attack 1
                if (Input.GetKeyDown(KeyCode.J))
                {
                    //attack for 10 damage with 0.5 seconds delay
                    attack(10, 0.5f);
                }

                //Jump, K right now
               if (Input.GetKeyDown(KeyCode.K))
               {
                   jump();
               }
            }
            playerEnergy = (int)Mathf.Round(audioMeasure.chargeAmount);
        }
    }

    //Movement script via Rigidbody 2D velocity
    public void moveCharacter(float hMove, float vMove)
    {
        if (canMove)
        {
            Vector3 targetV = new Vector2(hMove * horizontalSpeed, vMove * verticalSpeed);

            rigidBod.velocity = Vector3.SmoothDamp(rigidBod.velocity, targetV, ref velocity, movementSmooth);
            _anim.SetInteger("Walk", Mathf.RoundToInt(Mathf.Abs(horizontalMove + verticalMove)));
            if (hMove > 0 && !facingRight)
            {
                flip();
            }
            else if (hMove < 0 && facingRight)
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
    private void attack(int damage, float delayTime)
    {
        if (canAttack)
        {
            attackDamage = damage;
            //attackZone.enabled = true;
            canAttack = false;
            _anim.SetTrigger("isAttack");
            StartCoroutine(attackDelay(delayTime));

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
        if (canCharge)
        {
            if(!startCharging){
                startCharging = true;
            }
            canCharge = false;
            StartCoroutine(energyDelay());

            //Get Value from Microphone for charge float of 1-100 or 0-1
            //float energyScream = audioMeasure.movingAverage; //*value

            //if (playerEnergy < 100)
            //{
            //    playerEnergy += energyScream;
            //}
            //else
            //{
            //    playerEnergy = 100;
            //}

            if (playerEnergy < 100)
            {
                playerEnergy = (int)Mathf.Round(audioMeasure.chargeAmount);
                energyCharged += (int)Mathf.Round(audioMeasure.movingAverage);
            }
            else
            {
                playerEnergy = 100;
            }

            //Every 40 energy lay an egg
            if(energyCharged>=40)
            {
                //twea
                StartCoroutine(layEgg());
                energyCharged -= 40;
            }
        }
    }

    //Take Damage from enemy method
    private void hitOurHero(int damage)
    {
        if (canTakeDamage)
        {
            rigidBod.velocity = Vector2.zero;
            canTakeDamage = false;
            _anim.SetTrigger("isHurt");
            playerHealth -= damage;
            StartCoroutine(damageDelay());
            if (playerHealth <= 0)
            {
                isAlive = false;
                _anim.SetBool("isDead", true);
            }
        }
    }

    //Return attack damage for enemies hit by attack
    public int getDamage()
    {
        return attackDamage;

        //alternate return where energy effects attack damage
        /*
        float energy = (float)playerEnergy / 100f;
        float damage = attackDamage * energy;
        return (int)damage; */
    }


    public void ActivateAttackBox()
    {
        
        if (attackZone.isActiveAndEnabled)
            attackZone.enabled = false;
        else
            attackZone.enabled = true;

    }

    public void jump()
    {
        if (canJump)
        {
            canJump = false;
            _anim.SetTrigger("isJump");
            StartCoroutine(jumpDelay());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyAttackBox")
        {
            float damage = 10;
            //float damage = collision.gameObject.GetComponent<EnemyBaseClass>().attackDamage;
            hitOurHero((int)damage);
        }
    }

    IEnumerator attackDelay(float attackDelay)
    {
        yield return new WaitForSeconds(attackDelay);
        attackDamage = 0;
        //attackZone.enabled = false;
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

    IEnumerator jumpDelay()
    {
        yield return new WaitForSeconds(2.0f);
        canJump = true;
    }

    IEnumerator layEgg()
    {
        yield return new WaitForSeconds(1.0f);
        GameObject littleAttacker = Instantiate(egg);
        Vector3 eggPosition = transform.position;
        eggPosition.y = eggPosition.y - 3;
        littleAttacker.transform.position = eggPosition;
    }
}
