using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PatrolState", menuName = "FSM/States/Patrol", order = 2)]
public class PatrolState : AbstractFSMState
{
    private ConnectedWayPoint[] patrolPoints;
    private int patrolPointIndex;

    public override void OnEnable()
    {
        base.OnEnable();
        stateType = FSMState.PATROL;
        patrolPointIndex = -1;
    }

    public override bool EnterState()
    {
        if (base.EnterState()) 
        {
            EnteredState = false;
            //Get all the patrolpoints
            patrolPoints = executingNPC.PatrolPoints;

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
        if (EnteredState)
        {
            //Debug.Log("Patrol Point Index: " + patrolPointIndex);
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
            navMeshAgent.SetDestination(destinationPoint.transform.position);
        }
    }
}
