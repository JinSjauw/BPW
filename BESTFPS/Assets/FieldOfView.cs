using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    public float angle;

    public float radiusAlert;
    [Range(0, 360)]
    public float angleAlert;

    public float radiusDefault;
    [Range(0, 360)]
    public float angleDefault;


    public GameObject playerObject;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    private bool _playerSeen;
    public bool playerSeen 
    { get { return _playerSeen; } 
      set { if (_playerSeen == value) return;
            _playerSeen = value;
            if(OnPlayerSpotted != null) 
            {
                OnPlayerSpotted(_playerSeen);
            }
          }
    }
    public delegate void OnVariableChangeDelegate(bool newValue);
    public event OnVariableChangeDelegate OnPlayerSpotted;

    private void Start()
    {
        angle = angleDefault;
        radius = radiusDefault;
        StartCoroutine(TimedCheck());
    }

    // Start is called before the first frame update
    private IEnumerator TimedCheck() 
    {
        WaitForSeconds wait = new WaitForSeconds(0.3f);

        while (true) 
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2f)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    //Debug.Log("Can See Player");
                    playerSeen = true;
                    angle = angleAlert;
                    radius = radiusAlert;
                }
                else
                {
                    playerSeen = false;
                }
            }
            else { playerSeen = false; }
        }
        else if (playerSeen) 
        { 
            playerSeen = false;
            angle = angleDefault;
            radius = radiusDefault;
        }
    }

 



}
