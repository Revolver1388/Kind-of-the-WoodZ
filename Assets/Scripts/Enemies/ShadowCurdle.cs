using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCurdle : EnemyBaseClass
{
    public GameObject eggShootPrefab;
    public GameObject eggLayPrefab;
    public Transform shotEggloc;

    [SerializeField] float followSpeed = 2; //speed when following the player up and down 

    enum CurdleStates { follow, retreat, hit, attack, charge }
    CurdleStates curdleState = CurdleStates.follow;

    enum FacingDir { left, right }
    FacingDir currentlyFacing = FacingDir.left;

    enum MoveDir { vert, hor }
    MoveDir moveDirection = MoveDir.vert;

    Vector2 backApproach;
    Vector2 frontApproach;

    Coroutine charging;
    Coroutine hitting;

    private bool hasFired = false;
    private bool hasLaid = false;
    private Vector2 retreatTarget;

    public override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    private void OnEnable()
    {
        hasLaid = true;
        StartCoroutine(EggRecharge());
        StartCoroutine(ChargeReset());

        if (_parent.transform.position.x <= _player.transform.position.x)
        {
            _parent.transform.right = new Vector2(1, 0);
            currentlyFacing = FacingDir.left;
        }
        else
        {
            _parent.transform.right = new Vector2(-1, 0);
            currentlyFacing = FacingDir.right;
        }
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void FixedUpdate()
    {
        switch (curdleState)
        {
            case CurdleStates.follow:
                Follow();
                break;
            case CurdleStates.retreat:
                Retreat();
                break;
            case CurdleStates.hit:
                break;
            case CurdleStates.attack:
                break;
            case CurdleStates.charge:
                break;
        }

        
    }

    private void LayEgg()
    {
        GameObject eggL = Instantiate(eggLayPrefab, new Vector2(_parent.transform.position.x, _parent.transform.position.y), _parent.transform.rotation);
        StartCoroutine(EggRecharge());
        hasLaid = true;
        //all this will do is lay and egg
        //egg will instantiate another enemy when cracked
    }

    IEnumerator EggRecharge()
    {
        yield return new WaitForSeconds(Random.Range(10, 30));
        hasLaid = false;
    }


    private void Follow() //will move up and down the screen with the player, will not move closer though, if the player gets close enough then will melee attack
    {
        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, new Vector2(_parent.transform.position.x, _player.transform.position.y), followSpeed * Time.fixedDeltaTime);
        if(Mathf.Abs(_parent.transform.position.y - _player.transform.position.y) <= 1f)
        {
            if(Vector2.Distance(_parent.transform.position, _player.transform.position) <= 3)
            {
                AttackPlayer();
                StartCoroutine(AttackCD());
                curdleState = CurdleStates.attack;
            }
            else if(!hasFired)
            {
                ShootEgg();
                StartCoroutine(EggShootCoolDown());
                hasFired = true;
            }            
        }

        if (!hasLaid)
            LayEgg();

        if (currentlyFacing == FacingDir.right)
        {
            if (_parent.transform.position.x <= _player.transform.position.x)
            {
                _parent.transform.right = new Vector2(1, 0);
                currentlyFacing = FacingDir.left;
            }
        }
        else
        {
            if (_parent.transform.position.x > _player.transform.position.x)
            {
                _parent.transform.right = new Vector2(-1, 0);
                currentlyFacing = FacingDir.right;
            }
        }

        if (Mathf.Abs(_parent.transform.position.x - Camera.main.transform.position.x) > 15)
        {
            retreatTarget = new Vector2(Camera.main.transform.position.x + 13, _parent.transform.position.y);
        }        
        curdleState = CurdleStates.retreat;

        //RaycastHit2D hit;
        //hit = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + 0.5f), new Vector2(3, 1), 0, -transform.right, 3);   //(new Vector2(transform.position.x, transform.position.y + 0.5f), transform.right, 3);
        //if (hit)
        //{
        //    Debug.Log(hit.collider.gameObject.name);
        //    if (hit.collider.gameObject.GetComponent<EggScript>())
        //    {
        //        AttackPlayer();
        //    }
        //}
    }

    public void ShootEgg()
    {
        GameObject eggS = Instantiate(eggShootPrefab, shotEggloc);
        eggS.transform.parent = null;
        //eggS.transform.right = new Vector2(Mathf.Sign(_parent.transform.right.x), 0);
        //egg will move itself
        //need to set up egg to damage player
    }

    IEnumerator EggShootCoolDown()
    {
        yield return new WaitForSeconds(4);
        hasFired = false;
    }

    IEnumerator AttackCD()
    {
        yield return new WaitForSeconds(1.5f);
        if(_parent.transform.position.x <= Camera.main.transform.position.x)
        {
            retreatTarget = new Vector2(Camera.main.transform.position.x + 10, _parent.transform.position.y);
        }
        else 
        {
            retreatTarget = new Vector2(Camera.main.transform.position.x - 10, _parent.transform.position.y);
        }
        curdleState = CurdleStates.retreat;
    }

    IEnumerator ChargeReset()
    {
        yield return new WaitForSeconds(Random.Range(20, 40));
        {
            curdleState = CurdleStates.charge;
            _anim.SetBool("isCharge", true);
            charging = StartCoroutine(Charging());
        }
    }

    private void Retreat() // will run away from player to get to a safe distance
    {
        //retreat target will be dependant on camera position, so that shadowcurdle will not run off the screen
        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, retreatTarget, movementSpeed * Time.fixedDeltaTime);
        if(Vector2.Distance(_parent.transform.position, retreatTarget) < 1)
        {
            curdleState = CurdleStates.follow;
        }


        if (!hasLaid)
            LayEgg();
        
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        _anim.SetBool("isCharge", false);
        if(charging != null)
            StopCoroutine(charging);
        StartCoroutine(ChargeReset());
        if (hitting != null)
        {
            StopCoroutine(hitting);
        }
        hitting = StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.15f); //mayneed to change this based on animations
        if (_parent.transform.position.x <= Camera.main.transform.position.x)
        {
            retreatTarget = new Vector2(Camera.main.transform.position.x + 13, _parent.transform.position.y);
        }
        else
        {
            retreatTarget = new Vector2(Camera.main.transform.position.x - 13, _parent.transform.position.y);
        }
        curdleState = CurdleStates.retreat;       
    }

    IEnumerator Charging()
    {
        yield return new WaitForSeconds(5);
        Health += 50;
        if (Health > 500) //change this if max heath changes
            Health = 500;
        _anim.SetBool("isCharge", false);
        StartCoroutine(ChargeReset());
        curdleState = CurdleStates.follow;
    }


    public override void ActivateAttackBox()
    {
        base.ActivateAttackBox();
    }

    public override void AttackPlayer()
    {
        base.AttackPlayer();
    }

    public override void DeactivateEnemy()
    {
        base.DeactivateEnemy();
    }

    public override void ActivateHitBox()
    {
        base.ActivateHitBox();
    }

    public override void Die()
    {
        base.Die();
    }

    public override void EvadePlayer()
    {
        base.EvadePlayer();
    }

   
}
