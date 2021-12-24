using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectedWayPoint : WayPoint
{
    [SerializeField] protected float connectionRadius = 5f;

    List<ConnectedWayPoint> connections;

    public void Start()
    {
        GameObject[] allWayPoints = GameObject.FindGameObjectsWithTag("WayPoint");
        
        connections = new List<ConnectedWayPoint>();
    
        for(int i = 0; i < allWayPoints.Length; i++) 
        {
            ConnectedWayPoint nextWayPoint = allWayPoints[i].GetComponent<ConnectedWayPoint>();
            
            if(nextWayPoint != null) 
            {
                if(Vector3.Distance(transform.position, nextWayPoint.transform.position) <= connectionRadius && nextWayPoint != this) 
                {
                    connections.Add(nextWayPoint);
                } 
            }
        }
    }

    public override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, connectionRadius);
    }

    public ConnectedWayPoint NextWayPoint(ConnectedWayPoint previousWayPoint) 
    {
        if (connections.Count == 0)
        {
            return null;
        }
        else if (connections.Count == 1 && connections.Contains(previousWayPoint)) 
        {
            return previousWayPoint;
        }
        else 
        {
            ConnectedWayPoint nextWayPoint;
            int nextIndex = 0;

            do
            {
                nextIndex = UnityEngine.Random.Range(0, connections.Count);
                nextWayPoint = connections[nextIndex];
            } while (nextWayPoint == previousWayPoint);

            return nextWayPoint;
        }
    }

}
