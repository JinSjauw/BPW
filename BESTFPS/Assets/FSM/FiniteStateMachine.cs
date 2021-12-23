using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FiniteStateMachine : MonoBehaviour
    {
        AbstractFSMState previousState;
        AbstractFSMState currentState;
        
        [SerializeField] List<AbstractFSMState> validStates;
        Dictionary<FSMState, AbstractFSMState> fsmStates;

        public void Awake()
        {
            currentState = null;

            fsmStates = new Dictionary<FSMState, AbstractFSMState>();

            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            NPCController npc = GetComponent<NPCController>();

            foreach(AbstractFSMState state in validStates) 
            {
                state.SetExecutingFSM(this);
                state.SetExecutingNPC(npc);
                state.SetNavMeshAgent(navMeshAgent);
                //Debug.Log("Getting NavMesh Component in FiniteStateMachine: " + navMeshAgent.name);

                fsmStates.Add(state.stateType, state);
            }
        }

        public void Start()
        {
            EnterState(FSMState.IDLE);
        }

        private void Update()
        {
            if (currentState != null) 
            {
                currentState.UpdateState();
            }
        }

        // STATE MANAGEMENT

        public void EnterState(AbstractFSMState nextState) 
        {
            if (nextState == null) return;

            if(currentState != null) 
            {
            currentState.ExitState();
            }
            currentState = nextState;
            //Debug.Log("Current State:" + currentState);
            currentState.EnterState();
        }
        
        public void EnterState(FSMState state) 
        {   
            if (fsmStates.ContainsKey(state)) 
            {
                AbstractFSMState nextState = fsmStates[state];
                EnterState(nextState);
            }
        }

    }
