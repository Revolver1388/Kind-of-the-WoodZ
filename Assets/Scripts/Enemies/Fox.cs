using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : EnemyBaseClass
{
    enum FoxStates { postion, approach, attack, backoff, hit, dodge }
    FoxStates foxState = FoxStates.postion;

    enum FacingDir { left, right }
    FacingDir currentlyFacing = FacingDir.left;
    
    enum MoveDir { vert, hor }
    MoveDir moveDirection = MoveDir.vert;

    Vector2 approachTarget;

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
        switch(foxState)
        {
            case FoxStates.postion:
                Position();
                break;
            case FoxStates.approach:
                Approach();
                break;
            case FoxStates.attack:
                Attack();
                break;
            case FoxStates.backoff:
                BackOff();
                break;
            case FoxStates.hit:
                break;
            case FoxStates.dodge:
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

    private void Position()
    {
        if (moveDirection == MoveDir.vert)
        {
            if (_parent.transform.position.y - _player.transform.position.y <= 3 && _parent.transform.position.y >= _player.transform.position.y)
            {
                _parent.transform.position = new Vector2(_parent.transform.position.x, _parent.transform.position.y + movementSpeed * Time.fixedDeltaTime);
            }
            else if (_parent.transform.position.y - _player.transform.position.y >= -3 && _parent.transform.position.y <= _player.transform.position.y)
            {
                _parent.transform.position = new Vector2(_parent.transform.position.x, _parent.transform.position.y - movementSpeed * Time.fixedDeltaTime);
            }
            else
            {
                moveDirection = MoveDir.hor;
            }
        }
        else
        {
            //print(_player.transform.position.x - _player.transform.forward.normalized.x * 3);
            //_parent.transform.position = new Vector2( * Time.fixedDeltaTime, _parent.transform.position.y);
            Vector2 target = new Vector2(_player.transform.position.x - _player.transform.right.normalized.x * 5, _parent.transform.position.y);
            if (Vector2.Distance(_parent.transform.position, target) >= 0.5f)
            {
                _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, target, movementSpeed * Time.fixedDeltaTime);
            }
            else
            {
                approachTarget = new Vector2(_player.transform.position.x - _player.transform.right.normalized.x * 2.5f, _player.transform.position.y);
                foxState = FoxStates.approach;
            }
        }
        
        if (Vector2.Distance(_parent.transform.position, _player.transform.position) <= 2)
        {
            AttackPlayer();
            StartCoroutine(AttackWait());
            foxState = FoxStates.attack;
        }
    }

    private void Approach()
    {
        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, approachTarget, movementSpeed * Time.fixedDeltaTime);
        if(Vector2.Distance(_parent.transform.position, approachTarget) <= 0.1f)
        {
            if(Vector2.Distance(_parent.transform.position, _player.transform.position) >= 3)
            {
                foxState = FoxStates.postion;
            }
            else if(Vector2.Distance(_parent.transform.position, _player.transform.position) <= 2)
            {
                AttackPlayer();
                StartCoroutine(AttackWait());
                foxState = FoxStates.attack;
            }
            else
            {
                //if the player is facing the fox chance to dodge
                //else will attack
                if (Vector2.Dot(_player.transform.right, _player.transform.position - _parent.transform.position) < 0)
                {
                    int rando = Random.Range(0, 10);
                    if (rando >= 7)
                    {
                        foxState = FoxStates.dodge;
                        _hitBox.enabled = false;
                        StartCoroutine(DodgeWait());
                    }
                    else
                    {
                        AttackPlayer();
                        StartCoroutine(AttackWait());
                        foxState = FoxStates.attack;
                    }
                }
                else
                {
                    AttackPlayer();
                    StartCoroutine(AttackWait());
                    foxState = FoxStates.attack;
                }              
            }
        }
    }

    IEnumerator AttackWait()
    {
        yield return new WaitForSeconds(1.4f);
        foxState = FoxStates.backoff;
    }

    private void Attack()
    {
        //if(_anim.GetCurrentAnimatorStateInfo(0).IsName("Anim_Fox_Idle"))
        //{
        //    _anim.SetBool("isAttack", false);
        //    foxState = FoxStates.backoff;
        //}

    }

    private void BackOff()
    {
        _parent.transform.position = new Vector2(_parent.transform.position.x - movementSpeed * Time.fixedDeltaTime, _parent.transform.position.y);
        if (Vector2.Distance(_parent.transform.position, _player.transform.position) >= 6)
        {
            moveDirection = MoveDir.vert;
            foxState = FoxStates.postion;
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
        foxState = FoxStates.postion;
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
        foxState = FoxStates.hit;
        StopCoroutine(Hit());
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.6f);
        foxState = FoxStates.backoff;
    }
}
