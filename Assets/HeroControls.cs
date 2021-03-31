using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class HeroControls : MonoBehaviour
{
    [SerializeField] HitPopupController hitPopupController;

    //Stats
    [SerializeField] private int playerHealth = 6;
    [SerializeField] public int playerEnergy = 0;
    [SerializeField] public bool isAlive = true;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private bool canTakeDamage = true;
    private bool canCharge = true;
    [SerializeField] private GameObject[] heartCounter;


    private Transform cameraTransform;

    //Movement Variables
    public float horizontalSpeed = 8f;
    public float verticalSpeed = 4f;
    private Rigidbody2D rigidBod;
    [SerializeField] bool canMove = true;
    public bool facingRight = true;
    [Range(0, 1.0f)]
    [SerializeField] float movementSmooth = 0.1f;
    private Vector3 velocity = Vector3.zero;
    private float horizontalMove;
    private float verticalMove;
    [SerializeField] bool canJump;

    //Attack Variables
    [SerializeField] CircleCollider2D attackZone;
    [SerializeField] int attackDamage;
    private BoxCollider2D hitBox;

    //Charging Variables
    [SerializeField] private float energyCharged = 0;
    [SerializeField] private float startCharge = 0;
    private bool startCharging = false;
    [SerializeField] GameObject egg;

    //Animator
    Animator _anim;
    [SerializeField] GameObject chargeUp;
    SpriteRenderer _rend;

    //Microphone
    private AudioMeasure audioMeasure;


    private AudioManager audioManager;
    void Awake()
    {
        DOTween.Init();
        cameraTransform = Camera.main.transform;
        audioManager = FindObjectOfType<AudioManager>();
        audioMeasure = FindObjectOfType<AudioMeasure>();
        rigidBod = GetComponentInParent<Rigidbody2D>();
        attackZone = GetComponentInChildren<CircleCollider2D>();
        attackZone.enabled = false;
        checkHearts();
        hitBox = GetComponent<BoxCollider2D>();
        _anim = GetComponent<Animator>();
        attackDamage = 0;
        canJump = true;
        _rend = GetComponent<SpriteRenderer>();
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
                energyCharge();
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

                /*
                if(Input.GetKeyDown(KeyCode.L))
                {
                    pickUpHealth();
                }*/
            }
            playerEnergy = (int)Mathf.Round(audioMeasure.chargeAmount);
        }
    }

    public void PlayAudioClip(string audioClip)
    {
        audioManager.PlayOneShotByName(audioClip);
    }

    private void LateUpdate()
    {
        _rend.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
    }
    //Movement script via Rigidbody 2D velocity
    public void moveCharacter(float hMove, float vMove)
    {
        if (canMove)
        {
            Vector3 targetV = new Vector2(hMove * horizontalSpeed, vMove * verticalSpeed);

            rigidBod.velocity = Vector3.SmoothDamp(rigidBod.velocity, targetV, ref velocity, movementSmooth);
            _anim.SetInteger("Walk", Mathf.RoundToInt(Mathf.Abs(horizontalMove) + Mathf.Abs(verticalMove)));
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
            canCharge = false;
            if (!startCharging)
            {
                startCharging = true;
                startCharge = playerEnergy;
            }

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
                energyCharged = playerEnergy - startCharge;
            }
            else
            {
                playerEnergy = 100;
            }

            //Every 40 energy lay an egg
            if (energyCharged >= 20)
            {
                //twea
                StartCoroutine(layEgg());
                startCharge += 30;
            }
            canCharge = true;

        }
    }

    //Take Damage from enemy method
    private void hitOurHero(int damage)
    {
        if (canTakeDamage)
        {
            hitPopupController.SetHitAmount(damage);

            cameraTransform.DOShakePosition(0.1f, 0.5f, 50, 90, false, false);
            rigidBod.velocity = Vector2.zero;
            canTakeDamage = false;
            _anim.SetTrigger("isHurt");
            playerHealth -= damage;
            checkHearts();
            StartCoroutine(damageDelay());
            //updateHearts();
            if (playerHealth <= 0)
            {
                isAlive = false;
                _anim.SetBool("isDead", true);
                StartCoroutine(DeathHelper());
            }
        }
    }

    private IEnumerator DeathHelper()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadSceneAsync("Game_Over");
    }

    //Return attack damage for enemies hit by attack
    public int getDamage()
    {
        /*float energy = (float)playerEnergy / 100f;
        float damage = 5 + (attackDamage * energy);*/
        return playerEnergy;
    }

    private void pickUpHealth()
    {
        playerHealth++;
        checkHearts();
    }

    public void ActivateAttackBox()
    {

        if (attackZone.isActiveAndEnabled)
            attackZone.enabled = false;
        else
            attackZone.enabled = true;

    }

    public void JumpLayerSwap()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if(sprite.sortingLayerName=="WalkArea")
        {
            sprite.sortingLayerName = "JumpArea";
        }
        else
        {
            sprite.sortingLayerName = "WalkArea";
        }
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

    private void checkHearts()
    {
        if (playerHealth == 0)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);
        }
        else if(playerHealth == 1)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);

        }
        else if (playerHealth == 2)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);
        }
        else if (playerHealth == 3)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);
        }
        else if (playerHealth == 4)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);

        }
        else if (playerHealth == 5)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);
        }
        else if (playerHealth == 6)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);
        }
        else if (playerHealth == 7)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);
        }
        else if (playerHealth == 8)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", false);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);

        }
        else if (playerHealth == 9)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);
        }
        else if (playerHealth == 10)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", false);
        }
        else if (playerHealth == 11)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", true);
        }
        else if (playerHealth == 12)
        {
            heartCounter[0].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[1].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[2].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[3].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[4].GetComponent<Animator>().SetBool("isLife", true);
            heartCounter[5].GetComponent<Animator>().SetBool("isLife", true);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyAttackBox")
        {
            float damage = 1;
            //float damage = collision.gameObject.GetComponent<EnemyBaseClass>().attackDamage;
            hitOurHero((int)damage);
        }

        else if (collision.tag == "EggBox")
        {
            if(collision.gameObject.GetComponent<EggScript>().isEnemyEgg && !collision.gameObject.GetComponent<EggScript>().hasHit)
            {
                hitOurHero(1);
                collision.gameObject.GetComponent<EggScript>().eggCollision();
            }
        }
        else if (collision.tag == "HealthPack")
        {
            pickUpHealth();
        }
    }

    IEnumerator attackDelay(float attackDelay)
    {
        yield return new WaitForSeconds(attackDelay);
        attackDamage = 0;
        //attackZone.enabled = false;
        canAttack = true;
    }
    
    IEnumerator damageDelay()
    {
        yield return new WaitForSeconds(1.0f);
        canTakeDamage = true;
        //stop minor flashing animation?
    }

    IEnumerator jumpDelay()
    {
        yield return new WaitForSeconds(1.5f);
        canJump = true;
    }

    IEnumerator layEgg()
    {
        yield return new WaitForSeconds(1.0f);
        GameObject littleAttacker = Instantiate(egg);
        Vector3 eggPosition = transform.position;

        //move the egg slightly down
        eggPosition.y = eggPosition.y - Random.Range(0.000f, 2.000f);
        eggPosition.x = eggPosition.x - Random.Range(-4.00f, 4.00f);
        littleAttacker.transform.position = eggPosition;
    }
}
