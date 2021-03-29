// Created by Dylan LeClair 27/03/21
// Last modified 20/03/21 (Dylan LeClair)

using UnityEngine;

public class EnemyBaseClass : MonoBehaviour {
    [Header("Enemy Attributes")]
    [SerializeField] int Health = 0;
    [SerializeField] float movementSpeed = 0;
    [SerializeField] public float attackDamage = 0;
    [SerializeField] float attackSpeed = 0;
    CircleCollider2D _attackBox;
    BoxCollider2D _hitBox;
    HeroControls _player;

    bool isAlive = true;
    Animator _anim;
    void Awake(){
        _anim = GetComponent<Animator>();
        //player = GameManager.Instance.GetPlayerTransform();
        _attackBox = GetComponent<CircleCollider2D>();
        _hitBox = GetComponent<BoxCollider2D>();
        _player = FindObjectOfType<HeroControls>();
    }
    private void Start()
    {
        _attackBox.isTrigger = true;
        _attackBox.enabled = false;
        _hitBox.isTrigger = true;
    }
    void Update() {
        MoveTowardsPlayer();
    }

    public virtual void MoveTowardsPlayer() {

        transform.position = Vector2.MoveTowards(transform.position, _player.transform.position, movementSpeed * Time.deltaTime);
        // else if (Vector3.Distance(transform.position, _player.transform.position) <= 1) AttackPlayer();

    }

    public virtual void EvadePlayer() { }

    public virtual void AttackPlayer() {
        _anim.SetTrigger("isAttack");
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.CompareTag("PlayerHand") || collision.transform.CompareTag("PlayerFeet"))
            TakeDamage();
    }

    public void TakeDamage(){
        if (!isAlive)
            return;

        //Call player damage method
        int damage = _player.getDamage();
        Health -= damage;

        if(Health <= 0)
        {
            _anim.SetTrigger("isHurt");
            isAlive = false;
            Die();
        }
    }

    void Die() {
        _anim.SetBool("isDead", true);
      //Disbale colliders
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
}
