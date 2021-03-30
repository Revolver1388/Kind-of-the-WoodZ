using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : EnemyBaseClass
{
    enum BearStates { approach, attack, hit }
    BearStates bearState = BearStates.approach;

    enum FacingDir { left, right }
    FacingDir currentlyFacing = FacingDir.left;

    enum MoveDir { vert, hor }
    MoveDir moveDirection = MoveDir.vert;

    Vector2 backApproach;
    Vector2 frontApproach;

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
        }

    }

    private void Update()
    {
        print(currentlyFacing);
        if (currentlyFacing == FacingDir.right)
        {
            if (_parent.transform.position.x <= _player.transform.position.x)
            {
                print("infront");
                _parent.transform.right = new Vector2(-1, 0);
                currentlyFacing = FacingDir.left;
            }
        }
        else
        {
            print("facing left");
            if (_parent.transform.position.x > _player.transform.position.x)
            {
                print("behind");
                _parent.transform.right = new Vector2(1, 0);
                currentlyFacing = FacingDir.right;
            }
        }
    }

    //private void Position()
    //{
    //    if (moveDirection == MoveDir.vert)
    //    {
    //        if (_parent.transform.position.y - _player.transform.position.y <= 3 && _parent.transform.position.y >= _player.transform.position.y)
    //        {
    //            _parent.transform.position = new Vector2(_parent.transform.position.x, _parent.transform.position.y + movementSpeed * Time.fixedDeltaTime);
    //        }
    //        else if (_parent.transform.position.y - _player.transform.position.y >= -3 && _parent.transform.position.y <= _player.transform.position.y)
    //        {
    //            _parent.transform.position = new Vector2(_parent.transform.position.x, _parent.transform.position.y - movementSpeed * Time.fixedDeltaTime);
    //        }
    //        else
    //        {
    //            moveDirection = MoveDir.hor;
    //        }
    //    }
    //    else
    //    {
    //        //print(_player.transform.position.x - _player.transform.forward.normalized.x * 3);
    //        //_parent.transform.position = new Vector2( * Time.fixedDeltaTime, _parent.transform.position.y);
    //        Vector2 target = new Vector2(_player.transform.position.x - _player.transform.right.normalized.x * 5, _parent.transform.position.y);
    //        if (Vector2.Distance(_parent.transform.position, target) >= 0.3f)
    //        {
    //            _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, target, movementSpeed * Time.fixedDeltaTime);
    //        }
    //        else
    //        {
    //            approachTarget = new Vector2(_player.transform.position.x - _player.transform.right.normalized.x * 2.5f, _player.transform.position.y);
    //            bearState = BearStates.approach;
    //        }
    //    }
    //}

    private void Approach()
    {
        backApproach = new Vector2(_player.transform.position.x - 1f, _player.transform.position.y);
        frontApproach = new Vector2(_player.transform.position.x + 1f, _player.transform.position.y);
        if (Vector2.Distance(_parent.transform.position, backApproach) < Vector2.Distance(_parent.transform.position, frontApproach))
        {
            _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, backApproach, movementSpeed * Time.fixedDeltaTime);
            if (Vector2.Distance(_parent.transform.position, backApproach) <= 0.1f)
            {
                bearState = BearStates.attack;
            }
        }
        else
        {
            _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, frontApproach, movementSpeed * Time.fixedDeltaTime);
            if (Vector2.Distance(_parent.transform.position, backApproach) <= 0.1f)
            {
                bearState = BearStates.attack;
            }
        }

        
        //if (Vector2.Distance(_parent.transform.position, approachTarget) <= 0.1f)
        //{
        //    if (Vector2.Distance(_parent.transform.position, _player.transform.position) >= 3)
        //    {
        //        bearState = BearStates.postion;
        //    }
        //    else
        //    {
        //        //if the player is facing the fox chance to dodge
        //        //else will attack
        //        if (Vector2.Dot(_player.transform.right, _player.transform.position - _parent.transform.position) < 0)
        //        {
        //            int rando = Random.Range(0, 10);
        //            if (rando >= 7)
        //            {
        //                bearState = BearStates.dodge;
        //                _hitBox.enabled = false;
        //                StartCoroutine(DodgeWait());
        //            }
        //            else
        //            {
        //                AttackPlayer();
        //                StartCoroutine(AttackWait());
        //                bearState = BearStates.attack;
        //            }
        //        }
        //        else
        //        {
        //            AttackPlayer();
        //            StartCoroutine(AttackWait());
        //            bearState = BearStates.attack;
        //        }
        //    }
        //}
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(1.4f);
        //bearState = BearStates.backoff;
    }

    private void Attack()
    {        
        if(Vector2.Distance(_parent.transform.position, _player.transform.position) >= 2)
        {
            bearState = BearStates.approach;
        }
    }

    private void BackOff()
    {
        _parent.transform.position = new Vector2(_parent.transform.position.x - movementSpeed * Time.fixedDeltaTime, _parent.transform.position.y);
        if (Vector2.Distance(_parent.transform.position, _player.transform.position) >= 6)
        {
            moveDirection = MoveDir.vert;
            //bearState = BearStates.postion;
        }
    }

    private void Dodge()
    {
        _parent.transform.position = new Vector2(_parent.transform.position.x - movementSpeed * 3 * Time.fixedDeltaTime, _parent.transform.position.y);
    }

    IEnumerator DodgeWait()
    {
        yield return new WaitForSeconds(0.3f);
        _hitBox.enabled = true;
        //bearState = BearStates.postion;
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

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        int rando = Random.Range(0, 10);
        if (rando >= 7)
        {
            bearState = BearStates.hit;
            StopCoroutine(Hit());
            StartCoroutine(Hit());
        }

    }

    IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.6f);
        bearState = BearStates.attack;
    }
}
