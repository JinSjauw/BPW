using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(FiniteStateMachine), typeof(FieldOfView))]
[RequireComponent(typeof(IdleState), typeof(PatrolState))]
[RequireComponent(typeof(ChaseState), typeof(AttackState))]

public class NPCController : MonoBehaviour, IDamageAble
{
    public bool seesPlayer { get; private set; }

    [SerializeField] ConnectedWayPoint[] patrolPoints;

    NavMeshAgent navMeshAgent;
    FiniteStateMachine finiteStateMachine;
    FieldOfView fieldOfView;
    Animator animator;

    [Header("NPC Stats")]
    [SerializeField] int maxHP;
    int hp;
    [SerializeField] float aggroTimer = 5f;
    private float timer;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        finiteStateMachine = GetComponent<FiniteStateMachine>();
        fieldOfView = GetComponent<FieldOfView>();
        animator = GetComponentInChildren<Animator>();
        timer = aggroTimer;
        SetHP(maxHP);
    }

    public void Initialize()
    {
        finiteStateMachine.EnterState(FSMState.IDLE);
        fieldOfView.OnPlayerSpotted += PlayerSpotted;
    }

    public void Animate(string name, bool state) 
    {
        animator.SetBool(name, state);
    }

    public void Update()
    {
        if (!seesPlayer && finiteStateMachine.currentState.stateType == FSMState.CHASE) 
        {
            timer -= Time.deltaTime;
            if(timer < 0f) 
            {
                finiteStateMachine.EnterState(FSMState.IDLE);
                Debug.Log("Lost Aggro");
            }
        }
        else 
        {
            timer = aggroTimer;
        }
    }

    public void TakeDamage(int damage)
    {
        if(hp - damage <= 0) 
        {
            Die();
            return;
        }

        hp -= damage;
        Debug.Log("Took " + damage + " damage!");
        Debug.Log("Current HP: " + hp);
    }

    public void SetHP(int setHP)
    {
        hp = setHP;
    }

    public void Die()
    {
        //Destroy(this.gameObject);

        animator.enabled = false;
        finiteStateMachine.EnterState(FSMState.DEAD);
        //this.enabled = false;
    }

    public void Spawn() 
    {
        animator.enabled = true;
        SetHP(maxHP);
        finiteStateMachine.EnterState(FSMState.IDLE);
    }

    public ConnectedWayPoint[] PatrolPoints 
    {
        get { return patrolPoints; }
    }

    private void PlayerSpotted(bool isSpotted) 
    {
        seesPlayer = isSpotted;
    }
}
