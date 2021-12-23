using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(FiniteStateMachine))]
public class NPCController : MonoBehaviour
{
    [SerializeField] ConnectedWayPoint[] patrolPoints;

    NavMeshAgent navMeshAgent;
    FiniteStateMachine finiteStateMachine;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        finiteStateMachine = GetComponent<FiniteStateMachine>();
    }

    public ConnectedWayPoint[] PatrolPoints 
    {
        get { return patrolPoints; }
    }
}
