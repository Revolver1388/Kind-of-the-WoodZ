// Created by Dylan LeClair 27/03/21
// Last modified 20/03/21 (Dylan LeClair)

using UnityEngine;

public class EnemyBaseClass : MonoBehaviour {
    [Header("Enemy Attributes")]
    [SerializeField] int Health = 0;
    [SerializeField] float movementSpeed = 0;
    [SerializeField] float attackDamage = 0;
    [SerializeField] float attackSpeed = 0;
    [SerializeField] CircleCollider2D _attackBox;
    BoxCollider2D _hitBox;
    SpriteRenderer _rend;
    HeroControls _player;
    [SerializeField] Transform _layerOrd;
    bool isAlive = true;
    Animator _anim;
    void Awake(){
        _anim = GetComponent<Animator>();
        //player = GameManager.Instance.GetPlayerTransform();
        _attackBox = GetComponentInChildren<CircleCollider2D>();
        _hitBox = GetComponent<BoxCollider2D>();
        _rend = GetComponent<SpriteRenderer>();
        _player = FindObjectOfType<HeroControls>();
    }
    private void Start()
    {
        _attackBox.isTrigger = true;
        _attackBox.enabled = false;
        _hitBox.isTrigger = true;
    }
    void LateUpdate() {
        _rend.sortingOrder = Mathf.RoundToInt(_layerOrd.position.y) * -1;
    }

    public virtual void MoveTowardsPlayer() {


    }

    public virtual void EvadePlayer() { }

    public virtual void AttackPlayer() {
        _anim.SetTrigger("isAttack");
    }

    public void TakeDamage(){
        if (!isAlive)
            return;

        //Call player damage method
        _anim.SetTrigger("isHurt");
        if(Health > 0)
        {
            Health--;
        }
        if(Health <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    void Die() {
        _anim.SetBool("isDead", true);
    }

    public void DeactivateEnemy() {
        gameObject.SetActive(false);
    }

    public void ActivateAttackBox()
    {
        if (_attackBox.isActiveAndEnabled)
            _attackBox.enabled = false;
        else
            _attackBox.enabled = true;

    }
    public void ActivateHitBox()
    {
        if (_hitBox.isActiveAndEnabled)
            _hitBox.enabled = false;
        else
            _hitBox.enabled = true;

    }
    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.tag == "PlayerAttackBox") TakeDamage();
    }
}
