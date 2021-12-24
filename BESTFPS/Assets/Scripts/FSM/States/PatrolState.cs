using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "PatrolState", menuName = "FSM/States/Patrol", order = 2)]
public class PatrolState : AbstractFSMState
{
    private ConnectedWayPoint[] patrolPoints;
    private int patrolPointIndex;

    //private GameObject playerObject;
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = FSMState.PATROL;
        //Debug.Log("Agent Name: " + executingNPC.name);
    }

    private void Awake()
    {
        //playerObject = FindObjectOfType<RigidbodyController>().gameObject;
    }

    public override bool EnterState()
    {
        if (base.EnterState()) 
        {
            patrolPointIndex = -1;

            EnteredState = false;
            //Get all the patrolpoints
            patrolPoints = executingNPC.PatrolPoints;
            //Debug.Log("AGENT NAME: " + executingNPC.name);

            if(patrolPoints == null || patrolPoints.Length == 0) 
            {   
                Debug.LogError("PatrolState: Failed to get patrolpoints from npc");
                EnteredState = false;
                return EnteredState;
            }
            else 
            {
                if (patrolPointIndex < 0)
                {
                    patrolPointIndex = UnityEngine.Random.Range(0, patrolPoints.Length);
                }
                else
                {
                    //Wraps around the end of the array back to index 0
                    patrolPointIndex = (patrolPointIndex + 1) % patrolPoints.Length;
                }

                SetDestination(patrolPoints[patrolPointIndex]);
                EnteredState = true;
            }
        }
        return EnteredState;
    }

    public override void UpdateState()
    {
        //TODO 
        if (EnteredState && executionState == ExecutionState.ACTIVE)
        {
            //Debug.Log("Patrol Point Index: " + patrolPointIndex);
            if (executingNPC.seesPlayer) 
            {
                executingFSM.EnterState(FSMState.CHASE);
            }

            if(Vector3.Distance(navMeshAgent.transform.position, patrolPoints[patrolPointIndex].transform.position) <= 1f)
            {
                executingFSM.EnterState(FSMState.IDLE);
            }
        }
    }
    private void SetDestination(ConnectedWayPoint destinationPoint) 
    {
        if (navMeshAgent != null && destinationPoint != null) 
        {
            //Debug.Log("Agent Name: " + navMeshAgent.name + " Moving to: " + destinationPoint);
            navMeshAgent.SetDestination(destinationPoint.transform.position);
        }
    }
}
