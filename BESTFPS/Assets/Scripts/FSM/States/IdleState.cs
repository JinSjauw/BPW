using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "IdleState", menuName = "FSM/States/Idle", order = 1)]
public class IdleState : AbstractFSMState
{
    [SerializeField] private float idleTime = 3f;
    float totalTime;

    public override bool EnterState()
    {
        EnteredState = base.EnterState();

        if (EnteredState) 
        {
            executingNPC.Animate("isMoving", false);
            executingNPC.Animate("isShooting", false);
            navMeshAgent.isStopped = true;

            totalTime = 0f;
        }
        return EnteredState;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        stateType = FSMState.IDLE;

    }
    public override void UpdateState()
    {
        if (EnteredState && executionState == ExecutionState.ACTIVE) 
        {
            totalTime += Time.deltaTime;

            if(totalTime >= idleTime) 
            {
                executingFSM.EnterState(FSMState.PATROL);
            }
        }
    }
    public override bool ExitState()
    {
        base.ExitState();
        return true;
    }
}
