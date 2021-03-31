using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : EnemyBaseClass
{
    enum BearStates { approach, attack, hit, coolDown }
    BearStates bearState = BearStates.approach;

    enum FacingDir { left, right }
    FacingDir currentlyFacing = FacingDir.left;

    enum MoveDir { vert, hor }
    MoveDir moveDirection = MoveDir.vert;

    Vector2 backApproach;
    Vector2 frontApproach;

    Coroutine hitting;

    public override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    private void FixedUpdate()
    {
        switch (bearState)
        {           
            case BearStates.approach:
                Approach();
                break;
            case BearStates.attack:
                    Attack();
                break;           
            case BearStates.hit:
                break;
            case BearStates.coolDown:
                break;
        }

    }

    private void OnEnable()
    {
        if (_parent.transform.position.x <= _player.transform.position.x)
        {
            _parent.transform.right = new Vector2(-1, 0);
            currentlyFacing = FacingDir.left;
        }
        else if (_parent.transform.position.x > _player.transform.position.x)
        {
            _parent.transform.right = new Vector2(1, 0);
            currentlyFacing = FacingDir.right;
        }
    }

    private void Update()
    {
       
    }   

    private void Approach() //will move towards the player then attack when gets close enough
    {
        backApproach = new Vector2(_player.transform.position.x - 3f, _player.transform.position.y);
        frontApproach = new Vector2(_player.transform.position.x + 3f, _player.transform.position.y);
        if (Vector2.Distance(_parent.transform.position, backApproach) < Vector2.Distance(_parent.transform.position, frontApproach))
        {
            _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, backApproach, movementSpeed * Time.fixedDeltaTime);
            if (Vector2.Distance(_parent.transform.position, backApproach) <= 1f)
            {
                AttackPlayer();
                StartCoroutine(AttackWait());
                bearState = BearStates.attack;
            }
        }
        else
        {
            _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, frontApproach, movementSpeed * Time.fixedDeltaTime);
            if (Vector2.Distance(_parent.transform.position, frontApproach) <= 1f)
            {
                AttackPlayer();
                StartCoroutine(AttackWait());
                bearState = BearStates.attack;
            }
        }

        if (currentlyFacing == FacingDir.right)
        {
            if (_parent.transform.position.x <= _player.transform.position.x)
            {
                _parent.transform.right = new Vector2(-1, 0);
                currentlyFacing = FacingDir.left;
            }
        }
        else
        {
            if (_parent.transform.position.x > _player.transform.position.x)
            {
                _parent.transform.right = new Vector2(1, 0);
                currentlyFacing = FacingDir.right;
            }
        }
    }


    private void Attack() //bear will move forward slowly when attacking
    {
        //if (Vector2.Distance(_parent.transform.position, _player.transform.position) >= 1)
        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, new Vector2(_parent.transform.position.x + (5 * -Mathf.Sign(transform.right.x)), _parent.transform.position.y), movementSpeed / 2 * Time.fixedDeltaTime);
    } 

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        int rando = Random.Range(0, 10);
        if (rando >= 7)
        {
            bearState = BearStates.hit;
            if (hitting != null)
            {
                StopCoroutine(Hit());
            }
            hitting = StartCoroutine(Hit());
        }
    }


    //used to switch states
    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(1.5f);
        bearState = BearStates.coolDown;
        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(2);
        bearState = BearStates.approach;
    }

    IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.6f);
        bearState = BearStates.approach;
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
