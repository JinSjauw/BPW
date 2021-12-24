using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class AbilityHolder : MonoBehaviour
{
    //[SerializeField] InputAction key;
    //InputAction.CallbackContext context;
    private ObjectPool objectPool;

    public Ability ability;
    float cooldownTime;
    float activeTime;

    enum AbilityState 
    {
        READY,
        ACTIVE,
        COOLDOWN,
    }
    AbilityState state = AbilityState.READY;

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
    }

    public void OnThrow(InputValue context) 
    {
        Debug.Log("Throwing");
        if (context.isPressed && state == AbilityState.READY) 
        {
            ability.Activate(gameObject, objectPool);
            state = AbilityState.ACTIVE;
        }
    }

    private void Update()
    {
        if (ability == null) return;
        switch (state) 
        {
            case AbilityState.READY:
                //Debug.Log("Game Object: " + gameObject.name);
                state = AbilityState.READY;
                activeTime = ability.activeTime;
                break;
            case AbilityState.ACTIVE:
                if(activeTime > 0) 
                {
                    activeTime -= Time.deltaTime;
                }
                else 
                {
                    state = AbilityState.COOLDOWN;
                    cooldownTime = ability.cooldownTime;
                }
                break;
            case AbilityState.COOLDOWN:
                if(cooldownTime > 0) 
                {
                    cooldownTime -= Time.deltaTime;
                }
                else 
                {
                    state = AbilityState.READY;
                }
                break;
        }
    }
}
