using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : AbstractFSMState
{
    [SerializeField] private float respawnTimer = 10f;
    private float timer;

    [SerializeField] private Transform[] spawnPoints;
    private Collider mainCollider;
    private Collider[] allColliders;

    private void Awake()
    {
        mainCollider = GetComponent<Collider>();
        allColliders = GetComponentsInChildren<Collider>();
        timer = respawnTimer;
        DoRagdoll(false);

    }
    public override bool EnterState()
    {
        EnteredState = base.EnterState();

        if (EnteredState)
        {
            navMeshAgent.isStopped = true;
            DoRagdoll(true);
        }
        return EnteredState;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = FSMState.DEAD;

    }

    private void DoRagdoll(bool state) 
    {
        foreach (Collider col in allColliders)
        {
            col.enabled = state;
            if(col.GetComponent<Rigidbody>() != null) 
            {
                col.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
                col.GetComponent<Rigidbody>().isKinematic = !state;

            }
        }
        mainCollider.enabled = !state;
    }

    public override void UpdateState()
    {
        //Maybe respawn Timer
        if(EnteredState && executionState == ExecutionState.ACTIVE) 
        {
            timer -= Time.deltaTime;
            if(timer < 0f) 
            {
                DoRagdoll(false);

                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)].transform;
                executingNPC.transform.position = spawnPoint.position;
                executingNPC.Spawn();
                
                timer = respawnTimer;
            }
        }
    }

    public override bool ExitState()
    {
        base.ExitState();

        return true;
    }
}
