using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FiniteStateMachine : MonoBehaviour
    {
        public AbstractFSMState previousState { get; protected set; }
        public AbstractFSMState currentState { get; protected set; }
        public FSMState currentSTATE;
        

        //[SerializeField] List<AbstractFSMState> validStates;
        AbstractFSMState[] states;
        Dictionary<FSMState, AbstractFSMState> fsmStates;

    public void Awake()
    {
        /*currentState = null;

        fsmStates = new Dictionary<FSMState, AbstractFSMState>();
        states = GetComponents<AbstractFSMState>();
        Debug.Log("validStates: " + states.Length); 
        //Debug.Log("FSM : " + fsmStates + " Agent: " + this.gameObject.name);    

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        NPCController npc = GetComponent<NPCController>();

        for(int i = 0; i < states.Length; i++) 
        {
            states[i].SetExecutingFSM(this);
            states[i].SetExecutingNPC(npc);
            states[i].SetNavMeshAgent(navMeshAgent);
        //state.enabled = false;
        //Debug.Log(i);
        Debug.Log("Adding states: " + states[i] + " : " + states[i].stateType);

        if (!fsmStates.ContainsKey(states[i].stateType))
        {
            Debug.Log("Added: " + states[i].stateType);
            fsmStates.Add(states[i].stateType, states[i]);
        }
               
                
        }
    //Debug.Log("Getting NavMesh Component in FiniteStateMachine: " + this.name);
    Debug.Log("FSM STATES:" + fsmStates.Count);*/

    }

    public void Start()
    {
        currentState = null;

        fsmStates = new Dictionary<FSMState, AbstractFSMState>();
        states = GetComponents<AbstractFSMState>();
        Debug.Log("validStates: " + states.Length);
        //Debug.Log("FSM : " + fsmStates + " Agent: " + this.gameObject.name);    

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        NPCController npc = GetComponent<NPCController>();

        for (int i = 0; i < states.Length; i++)
        {
            states[i].SetExecutingFSM(this);
            states[i].SetExecutingNPC(npc);
            states[i].SetNavMeshAgent(navMeshAgent);
            //state.enabled = false;
            //Debug.Log(i);
            //Debug.Log("Adding states: " + states[i] + " : " + states[i].stateType);

            if (!fsmStates.ContainsKey(states[i].stateType))
            {
                //Debug.Log("Added: " + states[i].stateType);
                fsmStates.Add(states[i].stateType, states[i]);
            }

            npc.Initialize();
        }
        //Debug.Log("Getting NavMesh Component in FiniteStateMachine: " + this.name);
        //Debug.Log("FSM STATES:" + fsmStates.Count);
    }

    private void Update()
        {
            if (currentState != null) 
            {
                currentSTATE = currentState.stateType;
                currentState.UpdateState();
            }
        }

        // STATE MANAGEMENT

        public void EnterState(AbstractFSMState nextState) 
        {
        if (nextState == null) { Debug.Log("Next State is NULL"); return; }
            
        //Debug.Log("Current State:" + currentState );

        if (currentState != null) 
        {
                currentState.ExitState();
        }
        previousState = currentState;
        currentState = nextState;
        currentState.EnterState();
        }
        
        public void EnterState(FSMState state) 
        {
            //Debug.Log("Entering state: " + state + " Agent: " + this.gameObject.name);
            if (fsmStates.ContainsKey(state)) 
            {
            //Debug.Log("Entered State: " + state + " Agent " + this.gameObject.name);
                //Debug.Log("Found Key: " + state);
                AbstractFSMState nextState = fsmStates[state];
                
                EnterState(nextState);
            }
        }

    }
