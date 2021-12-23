using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum ExecutionState 
{
    NONE,
    ACTIVE,
    COMPLETED,
    TERMINATED,
};

public enum FSMState 
{
    IDLE,
    PATROL,
};

public abstract class AbstractFSMState : ScriptableObject
{
    protected NavMeshAgent navMeshAgent;
    protected NPCController executingNPC;
    protected FiniteStateMachine executingFSM;


    public ExecutionState executionState { get; protected set; }
    public FSMState stateType { get; protected set; }
    public bool EnteredState { get; protected set; }
    public virtual void OnEnable() 
    {
        executionState = ExecutionState.NONE;
    }
    public virtual bool EnterState() 
    {
        bool successNavMesh = true;
        bool successNPC = true;
        executionState = ExecutionState.ACTIVE;
        successNavMesh = (navMeshAgent != null);
        successNPC = (executingNPC != null);
        Debug.Log("Entering State: " + successNPC + " + " + successNavMesh);

        return successNPC & successNavMesh;
    }

    public abstract void UpdateState();

    public virtual bool ExitState() 
    {
        executionState = ExecutionState.COMPLETED;
        return true;
    }

    public virtual void SetNavMeshAgent(NavMeshAgent agent) 
    {
        //Debug.Log("Setting NavMEsh in AbstractFSMState: " + agent.name);

        if (agent != null)
        {
            navMeshAgent = agent;
        }
    }

    public virtual void SetExecutingFSM(FiniteStateMachine fsm)
    {
        if (fsm != null)
        {
            executingFSM = fsm;
        }
    }

    public virtual void SetExecutingNPC(NPCController npc) 
    {
        if(npc != null) 
        {
            executingNPC = npc;
        }
    }
}
