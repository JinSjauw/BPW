using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    private Transform muzzleTransform;
    public void setMuzzle(Transform muzzle) 
    {
        muzzleTransform = muzzle;
        transform.rotation = muzzleTransform.rotation;
    }

    public void setEnemyMuzzle(Transform muzzle)
    {
        muzzleTransform = muzzle;
        transform.rotation = muzzleTransform.rotation;
        transform.right = muzzle.forward;
    }

    private void Update()
    {
        transform.position = muzzleTransform.position;

    }
}
