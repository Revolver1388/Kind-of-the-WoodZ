// Created by Dylan LeClair 27/03/21
// Last modified 20/03/21 (Dylan LeClair)

using UnityEngine;

public class EnemyBaseClass : MonoBehaviour {
    [Header("Enemy Attributes")]
    [SerializeField] int Health = 0;
    [SerializeField] float movementSpeed = 0;
    [SerializeField] float attackDamage = 0;
    [SerializeField] float attackSpeed = 0;

    Transform player;

    bool isAlive = true;

    void Awake(){
        player = GameManager.Instance.GetPlayerTransform();
    }

    void Update() {
        
    }

    public virtual void MoveTowardsPlayer() { }

    public virtual void EvadePlayer() { }

    public virtual void AttackPlayer() { }

    void OnCollisionEnter(Collision collision){
        if (collision.transform.CompareTag("PlayerHand") || collision.transform.CompareTag("PlayerFeet"))
            TakeDamage();
    }

    public void TakeDamage(){
        if (!isAlive)
            return;

        //Call player damage method

        if(Health <= 0)
        {
            isAlive = false;
            Die();
        }
    }

    void Die() {
        //Play death animation
        //Disbale colliders
    }

    public void DeactivateEnemy() {
        gameObject.SetActive(false);
    }
}
