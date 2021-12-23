using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private GameObject ImpactPrefab;
    [SerializeField] private AudioClip impactSound;

    private Rigidbody bulletBody;
    private ObjectPool objectPool;

    private Vector3 lastPosition, currentPosition;
    private LayerMask DamageAbleLayer;

    private void Awake()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        bulletBody = GetComponent<Rigidbody>();
        DamageAbleLayer = LayerMask.GetMask("DamageAble");

    }
    private void OnEnable()
    {
        currentPosition = transform.position;
        //Debug.Log("Current Position: " + currentPosition + " transform positon: " + transform.position);
        StartCoroutine(TimeLife());
    }

    public void ShootBullet(Vector3 bulletVelocity, Transform muzzleTransform) 
    {
        transform.position = muzzleTransform.position;
        transform.forward = muzzleTransform.right;
        DetectCollision(muzzleTransform);
        bulletBody.velocity = bulletVelocity;
    }

    private void DetectCollision(Transform muzzleTransform) 
    {
        RaycastHit hit;
        if (Physics.Raycast(muzzleTransform.position, transform.forward, out hit, bulletBody.velocity.magnitude, DamageAbleLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green);
            Debug.Log("LineCasting");
            string tag = hit.collider.tag;
            switch (tag)
            {
                case "Enemy":
                    Debug.Log("Hit Enemy");
                    //Disable();
                    break;
                case "Enviroment":
                    Debug.Log("Hit Enviroment");
                    BulletImpact(hit.point);
                    //Disable();
                    break;
            }
        }
     /*   if(lastPosition != Vector3.zero) 
        {
            RaycastHit hit;
            if(Physics.Linecast(currentPosition, lastPosition, out hit, DamageAbleLayer))
            {
                Debug.Log("LineCasting");
                string tag = hit.collider.tag;
                switch (tag) 
                {
                    case "Enemy" :
                        Debug.Log("Hit Enemy");
                        Disable();
                        break;
                    case "Enviroment" :
                        Debug.Log("Hit Enviroment");
                        BulletImpact(hit.point);
                        Disable();
                        break;
                }
            }   
        }
        else 
        { 
            lastPosition = currentPosition;
        }*/
    }

    private void BulletImpact(Vector3 impactPoint) 
    {
        GameObject impactObject = objectPool.GetObject(ImpactPrefab);
        impactObject.transform.position = impactPoint;
        impactObject.transform.forward = -transform.forward;

        ImpactPrefab.SetActive(true);
        AudioManager.PlaySoundOnce(impactSound, impactObject.transform.position);
        
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
        lastPosition = Vector3.zero;
        if (objectPool != null)
        {
            objectPool.ReturnGameObject(this.gameObject);
        }
    }
    private void FixedUpdate()
    {
        //DetectCollision();
    }

}
