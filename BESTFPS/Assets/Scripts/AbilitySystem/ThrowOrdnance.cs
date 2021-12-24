using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ThrowOrdnance : Ability
{
    [SerializeField] private GameObject throwablePrefab;
    [SerializeField] private float throwVelocity;

    public override void Activate(GameObject holder, ObjectPool objectPool)
    {
        // Do shit here;
        Debug.Log("Object Pool: " + objectPool);
        GameObject grenade = objectPool.GetObject(throwablePrefab);
        grenade.transform.position = holder.transform.position;
        grenade.transform.rotation = Quaternion.AngleAxis(40, holder.transform.right);
        grenade.transform.position += new Vector3(0, 0, 3f);
        grenade.GetComponent<Rigidbody>().velocity = holder.transform.forward * throwVelocity;
    }
}
