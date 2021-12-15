using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    public float pickupRange;
    public float pickupRadius;

    public int weaponLayer;

    public Transform weaponHolder;
    public Transform playerCamera;

    private bool isWeaponHeld;
    private Weapon heldWeapon;

    private void Update() {
        if(isWeaponHeld)
        {
            
        } else {

        }
    }

    public void PickUpItem(InputAction.CallbackContext context)
    {
        Debug.Log("Picking Up");
        RaycastHit[] hitList = new RaycastHit[256];
        int hitNumber = Physics.CapsuleCastNonAlloc(playerCamera.position, 
        playerCamera.position + playerCamera.forward * pickupRange, pickupRadius, playerCamera.forward,
        hitList);

        List<RaycastHit> realList = new List<RaycastHit>();
        for(int i = 0; i < hitNumber; i++)
        {
            RaycastHit hit = hitList[i];
            if(hit.transform.gameObject.layer != weaponLayer) continue;
            if(hit.point == Vector3.zero)
            {
                realList.Add(hit);
            }
            else if(Physics.Raycast(playerCamera.position, hit.point - playerCamera.position, out RaycastHit hitInfo, hit.distance + 0.1f) && hitInfo.transform == hit.transform){
                realList.Add(hit);
            }
        }

        if(realList.Count == 0) return;

        realList.Sort((hit1, hit2) => {
            float dist1 = GetDistanceTo(hit1);
            float dist2 = GetDistanceTo(hit2);
            return Mathf.Abs(dist1 - dist2) < 0.001f ? 0 : dist1 < dist2 ? -1 : 1;
        });

        isWeaponHeld = true;
        heldWeapon = realList[0].transform.GetComponent<Weapon>();
        heldWeapon.PickUp(weaponHolder, playerCamera);
        
    }   

    private float GetDistanceTo(RaycastHit hit)
    {
        return Vector3.Distance(playerCamera.position, hit.point == Vector3.zero ? hit.transform.position : hit.point);
    }

      public void DropItem(InputAction.CallbackContext context)
    {
        Debug.Log("Dropping");
          if(isWeaponHeld)
        {
            heldWeapon.Drop(playerCamera);
            heldWeapon = null;
            isWeaponHeld = false;
        }
    }

}
