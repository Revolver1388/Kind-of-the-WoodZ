using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Racoon : EnemyBaseClass
{
    enum RacoonStates { postion, approach, attack, backoff, hit, dodge }
    RacoonStates racoonState = RacoonStates.postion;

    enum FacingDir { left, right }
    FacingDir currentlyFacing = FacingDir.left;

    enum MoveDir { vert, hor }
    MoveDir moveDirection = MoveDir.vert;

    Vector2 positionTarget;
    Vector2 approachTarget;

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

    public override void LateUpdate()
    {
        base.LateUpdate();
    }


    private void FixedUpdate()
    {
        switch (racoonState)
        {
            case RacoonStates.postion:
                Position();
                break;
            case RacoonStates.approach:
                Approach();
                break;
            case RacoonStates.attack:
                Attack();
                break;
            case RacoonStates.backoff:
                break;
            case RacoonStates.hit:
                break;
            case RacoonStates.dodge:
                Dodge();
                break;
        }

    }

    private void Update()
    {
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

    private void NewPosition()
    {
        int rando = Random.Range(0, 3);
        if(rando == 0)
        {
            positionTarget = new Vector2(_player.transform.position.x + 5, _player.transform.position.y + 2);
        }
        else if (rando == 1)
        {
            positionTarget = new Vector2(_player.transform.position.x + 5, _player.transform.position.y - 2);
        }
        else if (rando == 2)
        {
            positionTarget = new Vector2(_player.transform.position.x - 5, _player.transform.position.y - 2);
        }
        else if (rando == 3)
        {
            positionTarget = new Vector2(_player.transform.position.x - 5, _player.transform.position.y + 2);
        }
        racoonState = RacoonStates.postion;
    }

    private void Position() //will try to get behind player before moving in closer to attack
    {
        //if (moveDirection == MoveDir.vert)
        //{
        //    if (_parent.transform.position.y - _player.transform.position.y <= 3 && _parent.transform.position.y >= _player.transform.position.y)
        //    {
        //        _parent.transform.position = new Vector2(_parent.transform.position.x, _parent.transform.position.y + movementSpeed * Time.fixedDeltaTime);
        //    }
        //    else if (_parent.transform.position.y - _player.transform.position.y >= -3 && _parent.transform.position.y <= _player.transform.position.y)
        //    {
        //        _parent.transform.position = new Vector2(_parent.transform.position.x, _parent.transform.position.y - movementSpeed * Time.fixedDeltaTime);
        //    }
        //    else
        //    {
        //        moveDirection = MoveDir.hor;
        //    }
        //}
        //else
        //{
        //    Vector2 target = new Vector2(_player.transform.position.x - _player.transform.right.normalized.x * 5, _parent.transform.position.y);
        //    if (Vector2.Distance(_parent.transform.position, target) >= 0.5f)
        //    {
        //        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, target, movementSpeed * Time.fixedDeltaTime);
        //    }
        //    else
        //    {
        //        approachTarget = new Vector2(_player.transform.position.x - _player.transform.right.normalized.x * 2.5f, _player.transform.position.y);
        //        racoonState = RacoonStates.approach;
        //    }
        //}

        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, positionTarget, movementSpeed * Time.fixedDeltaTime);
        if (Vector2.Distance(_parent.transform.position, positionTarget) <= 0.1f)
        {
            racoonState = RacoonStates.approach;
        }


        //if (Vector2.Distance(_parent.transform.position, _player.transform.position) <= 2)
        //{
        //    AttackPlayer();
        //    StartCoroutine(AttackWait());
        //    racoonState = RacoonStates.attack;
        //}
    }

    private void Approach() //approaches player from behind then will attack when close enough
    {
        if(_parent.transform.position.x < _player.transform.position.y)
        {
            approachTarget = new Vector2(_player.transform.position.x - 1f, _player.transform.position.y);
        }
        else
        {
            approachTarget = new Vector2(_player.transform.position.x - 1f, _player.transform.position.y);

        }
        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, approachTarget, movementSpeed * Time.fixedDeltaTime);

        if(Vector2.Distance(_parent.transform.position, approachTarget) <= 0.1f)
        {
            AttackPlayer();
            StartCoroutine(AttackWait());
            racoonState = RacoonStates.attack;
        }       
    }



    private void Attack()
    {
        //if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Anim_Fox_Idle"))
        //{
        //    _anim.SetBool("isAttack", false);
        //    foxState = FoxStates.backoff;
        //}
    }    

    private void Dodge()
    {
        _parent.transform.position = new Vector2(_parent.transform.position.x - movementSpeed * 5 * Time.fixedDeltaTime, _parent.transform.position.y);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        racoonState = RacoonStates.hit;
        if (hitting != null)
        {
            StopCoroutine(Hit());
        }
        hitting = StartCoroutine(Hit());
    }

    //waits to change states
    IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.6f);
        racoonState = RacoonStates.postion;
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(1.4f);
        racoonState = RacoonStates.postion;
    }

    IEnumerator DodgeWait()
    {
        yield return new WaitForSeconds(0.3f);
        _hitBox.enabled = true;
        racoonState = RacoonStates.postion;
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
