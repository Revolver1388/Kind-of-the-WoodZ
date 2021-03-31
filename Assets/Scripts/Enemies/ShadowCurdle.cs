using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCurdle : EnemyBaseClass
{
    public GameObject eggShootPrefab;
    public GameObject eggLayPrefab;

    [SerializeField] float followSpeed = 2; //speed when following the player up and down 

    enum CurdleStates { follow, retreat, hit}
    CurdleStates curdleState = CurdleStates.follow;

    enum FacingDir { left, right }
    FacingDir currentlyFacing = FacingDir.left;

    enum MoveDir { vert, hor }
    MoveDir moveDirection = MoveDir.vert;

    Vector2 backApproach;
    Vector2 frontApproach;

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
        }

        if (!hasLaid)
            LayEgg();
    }

    private void LayEgg()
    {
        GameObject eggL = Instantiate(eggShootPrefab, new Vector2(_parent.transform.position.x, _parent.transform.position.y), _parent.transform.rotation);
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


    private void Follow() //will move up and down the screen with the player, will not move closer though
    {
        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, new Vector2(_parent.transform.position.x, _player.transform.position.y), followSpeed * Time.fixedDeltaTime);
        if(Mathf.Abs(_parent.transform.position.y - _player.transform.position.y) <= 0.5f && !hasFired)
        {
            ShootEgg();
            StartCoroutine(EggShootCoolDown());
            hasFired = true;
        }
    }

    public void ShootEgg()
    {
        GameObject eggS = Instantiate(eggShootPrefab, new Vector2(_parent.transform.position.x, _parent.transform.position.y), _parent.transform.rotation);
        //egg will move itself
        //need to set up egg to damage player
    }

    IEnumerator EggShootCoolDown()
    {
        yield return new WaitForSeconds(2);
        hasFired = false;
    }

    private void Retreat() // will run away from player to get to a safe distance
    {
        //retreat target will be dependant on camera position, so that shadowcurdle will not run off the screen
        _parent.transform.position = Vector2.MoveTowards(_parent.transform.position, retreatTarget, movementSpeed * Time.fixedDeltaTime);
    }


    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        StopCoroutine(Hit());
        StartCoroutine(Hit());
    }

    IEnumerator Hit()
    {
        yield return new WaitForSeconds(0.6f); //mayneed to change this based on animations
        curdleState = CurdleStates.retreat;
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
