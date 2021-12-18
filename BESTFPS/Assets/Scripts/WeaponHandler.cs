using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Implement Weapon Sway, Switching weapon states(ADS/HIP)
public class WeaponHandler : MonoBehaviour
{
    private Weapon weapon;
    public Weapon heldWeapon{set { weapon = value; }}

    private void OnFire(InputValue context)
    {
        if(context.isPressed && weapon != null)
        {
            weapon.UpdateState(1);

        }else if (!context.isPressed && weapon != null) 
        {
            weapon.UpdateState(0);
        }
    }

    private void OnReload(InputValue context)
    {
        if(context.isPressed && weapon != null) 
        {
            weapon.Reload();
        }
    }
}
