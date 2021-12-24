using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : AbstractFSMState
{
    public GameObject playerObject;
    [SerializeField] int totalEnemies = 15;
    [SerializeField] float alertRadius = 7f;
    LayerMask enemyLayer;


    public override void OnEnable()
    {
        base.OnEnable();
        stateType = FSMState.CHASE;
    }

    private void Awake()
    {
        playerObject = FindObjectOfType<RigidbodyController>().gameObject;
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    public override bool EnterState()
    {
        //stateType = FSMState.CHASE;
        if (base.EnterState())
        {
            EnteredState = false;
            //Do stuff
            if (playerObject != null)
            {
                navMeshAgent.isStopped = false;
                AlertAllies();
            }
            else 
            {
                executingFSM.EnterState(FSMState.IDLE);
            }
            EnteredState = true;
        }
        //executingNPC.Animate("isMoving", true);
        return EnteredState;
    }

    public override void UpdateState()
    {
        if (EnteredState && executionState == ExecutionState.ACTIVE) 
        {
            if (playerObject != null)
            {
                if (executingNPC.seesPlayer)
                {
                    //AlertAllies();
                    Debug.Log("Entering ATTACK state from CHASE");
                    executingFSM.EnterState(FSMState.ATTACK);
                }
                else 
                {
                    SetDestination(playerObject.transform.position);
                }
            }
        }
    }

    public override bool ExitState()
    {
        return base.ExitState();
    }

    private void AlertAllies() 
    {
        Debug.Log("Alerting Allies " + this.name);
        Collider[] enemies = new Collider[50];

        int indexColliders = Physics.OverlapSphereNonAlloc(transform.position, alertRadius, enemies , enemyLayer);
        for(int i = 0; i < indexColliders; i++)
        {
            if (enemies[i] != null) return;
            FiniteStateMachine enemyFSM = enemies[i].GetComponent<FiniteStateMachine>();
            Debug.Log(enemyFSM.name);
            enemyFSM.EnterState(FSMState.CHASE);
        }
    }

    private void SetDestination(Vector3 destination) 
    {
        executingNPC.Animate("isMoving", true);
        navMeshAgent.SetDestination(destination);
    }
}