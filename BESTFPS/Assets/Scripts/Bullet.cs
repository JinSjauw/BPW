using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;

    private Rigidbody bulletBody;
    private ObjectPool objectPool;

    private Vector3 lastPosition;
    private LayerMask enviromentLayer;

    private void Awake()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        bulletBody = GetComponent<Rigidbody>();
        enviromentLayer = LayerMask.GetMask("Enviroment");
    }
    private void OnEnable()
    {
        StartCoroutine(TimeLife());
    }

    public void ShootBullet(Vector3 bulletVelocity, Transform muzzleTransform) 
    {
        bulletBody.velocity = bulletVelocity;
        transform.position = muzzleTransform.position;
        transform.forward = muzzleTransform.right;
    }

    private void DetectCollision() 
    {
        Vector3 currentPosition = transform.position;
        if(lastPosition != null) 
        {
            RaycastHit hit;
            if(Physics.Linecast(currentPosition, lastPosition, out hit, enviromentLayer))
            {
                if(hit.collider.tag == "Enviroment") 
                {
                    Debug.Log("Hit Enviroment");
                    Disable();
                }
            }
        }
        else 
        { 
            lastPosition = currentPosition;
        }
    }
    private IEnumerator TimeLife() 
    {
        yield return new WaitForSeconds(lifeTime);
        Disable();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enviroment")
        {
            Disable();
        }
    }
    private void Disable() 
    {
        if (objectPool != null)
        {
            objectPool.ReturnGameObject(this.gameObject);
        }
    }
    private void FixedUpdate()
    {
        DetectCollision();
    }

}
