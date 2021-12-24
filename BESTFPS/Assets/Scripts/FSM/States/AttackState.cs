using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AbstractFSMState
{
    private GameObject playerObject;
    private ObjectPool objectPool;

    [SerializeField] GameObject bulletPrefab, muzzleTransform;
    [SerializeField] private float fireRate, attackDelay, muzzleVelocity;
    private bool canShoot, canAttack = true;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = FSMState.ATTACK;
    }

    private void Awake()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        playerObject = FindObjectOfType<RigidbodyController>().gameObject;
        canShoot = true;
    }

    public override bool EnterState()
    {
        //stateType = FSMState.ATTACK;

        if (base.EnterState()) 
        {
            EnteredState = false;
            Debug.Log("IN ATTACK STATE");
            navMeshAgent.isStopped = true;
            EnteredState = true;
        }
        return EnteredState;
    }

    public override void UpdateState()
    {
        if (EnteredState && executionState == ExecutionState.ACTIVE) 
        {
            if (executingNPC.seesPlayer && executingFSM.currentState.stateType == FSMState.ATTACK)
            {
                transform.LookAt(playerObject.transform.position);
                if (canAttack)
                {
                    StartCoroutine(Attack());
                }
            }
            else if (!executingNPC.seesPlayer && executingFSM.currentState.stateType != FSMState.CHASE)
            {
                navMeshAgent.isStopped = false;
                executingFSM.EnterState(FSMState.CHASE);
            }
        }
    }
    private void Shoot() 
    {
        //Debug.Log(objectPool);
        if (!canShoot) return;
        GameObject bulletObject = objectPool.GetObject(bulletPrefab);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.EnemyShootBullet((playerObject.transform.position - transform.position).normalized * muzzleVelocity, muzzleTransform.transform);
        StartCoroutine(FiringCooldown());
    }

    private IEnumerator Attack() 
    {
        canAttack = false;
        yield return new WaitForSeconds(2f);
        canAttack = true;
        Shoot();
    }
    private IEnumerator FiringCooldown() 
    {
        canShoot = false;
        yield return new WaitForSeconds(60 / fireRate);
        canShoot = true;
    }
}
