using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : EnemyBaseClass
{
    enum FoxStates { postion, approach, attack, backoff, hit }
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
                break;
            case FoxStates.backoff:
                break;
            case FoxStates.hit:
                break;
        }

    }

    private void Update()
    {
        //print(currentlyFacing);
        if (currentlyFacing == FacingDir.right)
        {
            if (_parent.transform.position.x <= _player.transform.position.x)
            {
                //print("infront");
                _parent.transform.right = new Vector2(-1, 0);
                currentlyFacing = FacingDir.left;
            }
        }
        else
        {
            //print("facing left");
            if (_parent.transform.position.x > _player.transform.position.x)
            {
                //print("behind");
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
            if (Vector2.Distance(_parent.transform.position, target) >= 0.3f)
            {
                _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, target, movementSpeed * Time.fixedDeltaTime);
            }
            else
            {
                approachTarget = new Vector2(_player.transform.position.x - _player.transform.right.normalized.x * 2.5f, _player.transform.position.y);
                foxState = FoxStates.approach;
            }
        }         
    }

    private void Approach()
    {
        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, approachTarget, movementSpeed * Time.fixedDeltaTime);
        if(Vector2.Distance(_parent.transform.position, approachTarget) <= 0.1f)
        {
            if(Vector2.Distance(_player.transform.position, _player.transform.position) >= 3)
            {
                foxState = FoxStates.postion;
            }
            else
            {
                //if the player is facing the fox chance to dodge
                //else will attack
                AttackPlayer();
            }
        }
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

    public override void TakeDamage()
    {
        base.TakeDamage();
    }
}
