using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Implement Weapon Sway, Switching weapon states(ADS/HIP)
public class WeaponHandler : MonoBehaviour
{
    private Weapon weapon;
    public Weapon heldWeapon{set { weapon = value; }}
    private Animator weaponAnimator;
    private Transform hipPos, adsPos;
    private bool isAiming;
    private enum AimMode { HIP, ADS }
    private AimMode aimMode;

    private void Awake()
    {
        hipPos = gameObject.transform.Find("HIP");
        adsPos = gameObject.transform.Find("ADS");
    }

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

    private void OnAim(InputValue context) 
    {

        if(context.isPressed && weapon != null) 
        {
            aimMode = AimMode.ADS;
        }else if(!context.isPressed && weapon != null) 
        {
            aimMode = AimMode.HIP;
        }
    }

    private void Aim() 
    {
        if (weapon == null) return;

        Transform weaponTransform = weapon.transform;
        if (aimMode == AimMode.ADS) 
        {
            weapon.Aim(true);
            weapon.transform.position = Vector3.Lerp(weaponTransform.position, adsPos.position, 5f * Time.deltaTime);
        }else if (aimMode == AimMode.HIP) 
        {
            weapon.Aim(false);
            weapon.transform.position = Vector3.Lerp(weaponTransform.position, hipPos.position, 5f * Time.deltaTime);
        }
    }

    private void Update()
    {
        Aim();
    }


}
