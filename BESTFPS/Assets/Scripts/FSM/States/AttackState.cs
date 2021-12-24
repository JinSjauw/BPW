using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : AbstractFSMState
{
    private GameObject playerObject;
    private ObjectPool objectPool;

    [SerializeField] GameObject bulletPrefab, muzzleTransform, muzzleFlashPrefab;
    [SerializeField] AudioClip shootSound;
    [SerializeField] private float fireRate, attackDelay, muzzleVelocity;
    [SerializeField] private int weaponDamage;
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
        fireRate = Random.Range(300f, 800f);
        attackDelay = Random.Range(0.2f, 1f);
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
                Vector3 point = playerObject.transform.position;
                muzzleTransform.transform.LookAt(point);
                //point.y = 0.0f; 
                transform.LookAt(point);
                executingNPC.Animate("isMoving", false);
                if (canAttack)
                {
                    StartCoroutine(Attack());
                }
                executingNPC.Animate("isShooting", false);

            }
            else if (!executingNPC.seesPlayer && executingFSM.currentState.stateType != FSMState.CHASE)
            {
                navMeshAgent.isStopped = false;
                executingNPC.Animate("isShooting", false);
                executingFSM.EnterState(FSMState.CHASE);
            }
        }
    }
    private void Shoot() 
    {
        //Debug.Log(objectPool);
        if (!canShoot) return;
        executingNPC.Animate("isShooting", true);
        GameObject bulletObject = objectPool.GetObject(bulletPrefab);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.setDamage(weaponDamage);
        bullet.EnemyShootBullet((playerObject.transform.position - muzzleTransform.transform.position).normalized * muzzleVelocity, muzzleTransform.transform);

        GameObject muzzleFlashObject = objectPool.GetObject(muzzleFlashPrefab);
        MuzzleFlash muzzleFlash = muzzleFlashObject.GetComponent<MuzzleFlash>();
        muzzleFlash.setEnemyMuzzle(muzzleTransform.transform);

        AudioManager.PlaySoundOnce(shootSound, muzzleTransform.transform.position);

        StartCoroutine(FiringCooldown());
    }

    private IEnumerator Attack() 
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
        Shoot();
    }
    private IEnumerator FiringCooldown() 
    {
        canShoot = false;
        yield return new WaitForSeconds(60 / fireRate);
        canShoot = true;
    }

    public override bool ExitState()
    {
        executingNPC.Animate("isShooting", false);
        return base.ExitState();
    }

}
