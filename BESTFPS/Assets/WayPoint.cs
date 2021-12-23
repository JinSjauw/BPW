using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private float debugDrawRadius = 1.0f;

    public virtual void OnDrawGizmos() 
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, debugDrawRadius);
    }
}
