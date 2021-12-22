using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Implement Weapon Sway, Switching weapon states(ADS/HIP)
public class WeaponHandler : MonoBehaviour
{
    private Weapon weapon;
    private Recoil recoilHandler;
    public Weapon heldWeapon{set { weapon = value; }}
    private Transform hipPos, adsPos;
    private bool isAiming;
    private enum AimMode { HIP, ADS }
    private AimMode aimMode;
    private void Awake()
    {
        hipPos = gameObject.transform.Find("HIP");
        adsPos = gameObject.transform.Find("ADS");
        recoilHandler = GetComponentInParent<Recoil>();
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

    private void OnMove(InputValue context)
    {
        if (weapon == null) return;
        if(context.Get<Vector2>() != Vector2.zero) 
        {
            weapon.Animate("Walking", true);
        }
        else 
        {
            weapon.Animate("Walking", false);
        }
    }

    private void OnSprint(InputValue context) 
    {
        if (weapon == null) return;
        if (context.isPressed) 
        {
            weapon.Animate("Sprinting", true);
        }else 
        {
            weapon.Animate("Sprinting", false);
        }
    }

    private void OnAim(InputValue context) 
    {
        if (weapon == null) return;
        if(context.isPressed) 
        {
            aimMode = AimMode.ADS;
        }else if(!context.isPressed) 
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
            weapon.Animate("Aiming", true);
            weapon.transform.position = Vector3.Lerp(weaponTransform.position, adsPos.position, 5f * Time.deltaTime);
            recoilHandler.SetRecoil(weapon.adsRecoil);
        }else if (aimMode == AimMode.HIP) 
        {
            weapon.Animate("Aiming", false);
            weapon.transform.position = Vector3.Lerp(weaponTransform.position, hipPos.position, 5f * Time.deltaTime);
            recoilHandler.SetRecoil(weapon.hipRecoil);
        }
    }

    private void Update()
    {
        Aim();
    }
}
