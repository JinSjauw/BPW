using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Health : MonoBehaviour, IDamageAble
{
    [SerializeField] Camera deathCamera;
    [SerializeField] private int maxHp = 100;
    private int totalHP;


    private void Awake() 
    {
        SetHP(maxHp);
    }
    public void Die()
    {
        this.gameObject.GetComponentInParent<PlayerInput>().gameObject.SetActive(false);
        deathCamera.enabled = true;
        //this.gameObject.SetActive(false);
    }

    public void SetHP(int setHP)
    {
        totalHP = setHP;
    }

    public void TakeDamage(int damage)
    {
        if(totalHP - damage < 0) 
        { 
            Die(); return; 
        }
        totalHP -= damage;
    }
}
