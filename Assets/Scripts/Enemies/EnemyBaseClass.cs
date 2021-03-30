﻿// Created by Dylan LeClair 27/03/21
// Last modified 20/03/21 (Dylan LeClair)

using UnityEngine;

public class EnemyBaseClass : MonoBehaviour {
    [Header("Enemy Attributes")]
    [SerializeField] int Health = 0;
    public float movementSpeed = 0;
    [SerializeField] float attackDamage = 0;
    [SerializeField] float attackSpeed = 0;
    [SerializeField] CircleCollider2D _attackBox;
    public BoxCollider2D _hitBox;
    SpriteRenderer _rend;
    public HeroControls _player;
    public GameObject _parent;
    [SerializeField] Transform _layerOrd;
    bool isAlive = true;
    public Animator _anim;
    public virtual void Awake(){
        _anim = GetComponent<Animator>();
        //player = GameManager.Instance.GetPlayerTransform();
        _attackBox = GetComponentInChildren<CircleCollider2D>();
        _hitBox = GetComponent<BoxCollider2D>();
        _rend = GetComponent<SpriteRenderer>();
        _player = FindObjectOfType<HeroControls>();
        _parent = transform.parent.gameObject;
    }
    public virtual void Start()
    {
        _attackBox.isTrigger = true;
        _attackBox.enabled = false;
        _hitBox.isTrigger = true;
    }

    public virtual void LateUpdate() {
        _rend.sortingOrder = Mathf.RoundToInt(_layerOrd.position.y) * -1;
    }

    public virtual void MoveTowardsPlayer() {


    }

    public virtual void EvadePlayer() { }

    public virtual void AttackPlayer() {
        _anim.SetTrigger("isAttack");
    }

    public virtual void TakeDamage(int damage){
        if (!isAlive)
            return;

        //Call player damage method
        _anim.SetTrigger("isHurt");
        if(Health > 0)
        {
            Health -= damage;
        }
        if(Health <= 0)
        {
            isAlive = false;
            this.enabled = false;
            Die();
        }
    }

    public virtual void Die() {
        _anim.SetBool("isDead", true);
    }

    public virtual void DeactivateEnemy() {
        gameObject.SetActive(false);
    }

    public virtual void ActivateAttackBox()
    {
        if (_attackBox.isActiveAndEnabled)
            _attackBox.enabled = false;
        else
            _attackBox.enabled = true;

    }
    public virtual void ActivateHitBox()
    {
        if (_hitBox.isActiveAndEnabled)
            _hitBox.enabled = false;
        else
            _hitBox.enabled = true;

    }

    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.tag == "PlayerAttackBox")
        {
            if (_player.playerEnergy < 10)
            {
                TakeDamage(10);
            }
            else
                TakeDamage(_player.playerEnergy);
        }
    }
}
