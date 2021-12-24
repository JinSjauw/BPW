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
        //Debug.Log("Entered State? in IdleState: " + EnteredState);
        EnteredState = base.EnterState();
       // Debug.Log("Entered State? in IdleState: " + EnteredState);

        if (EnteredState) 
        {
            //Debug.Log("Entered IDLE State: " + executingNPC.name);
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
            //Debug.Log("Updating IDLE State: " + totalTime);

            if(totalTime >= idleTime) 
            {
                executingFSM.EnterState(FSMState.PATROL);
            }
        }
    }
    public override bool ExitState()
    {
        base.ExitState();

        //Debug.Log("Exiting IDLE State");
        return true;
    }
}
