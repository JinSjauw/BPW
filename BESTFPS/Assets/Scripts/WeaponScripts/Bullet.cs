using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private GameObject ImpactEnviromentPrefab;
    [SerializeField] private GameObject ImpactFleshPrefab;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private int bulletDamage;

    private Rigidbody bulletBody;
    private ObjectPool objectPool;

    private Vector3 lastPosition, currentPosition;
    private LayerMask DamageAbleLayer;

    private void Awake()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        bulletBody = GetComponent<Rigidbody>();

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
        bulletBody.velocity = bulletVelocity;
        DamageAbleLayer = LayerMask.GetMask("DamageAble", "Enemy");
        DetectCollision(muzzleTransform.position);
    }
    public void EnemyShootBullet(Vector3 bulletVelocity, Transform muzzleTransform)
    {
        transform.position = muzzleTransform.position;
        transform.forward = muzzleTransform.forward;
        bulletBody.velocity = bulletVelocity;
        DamageAbleLayer = LayerMask.GetMask("DamageAble", "Player");
        DetectCollision(muzzleTransform.position);
    }

    private void DetectCollision(Vector3 muzzlePosition) 
    {
        RaycastHit hit;
        if (Physics.Raycast(muzzlePosition, transform.forward, out hit, bulletBody.velocity.magnitude, DamageAbleLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.green);
            //Debug.Log("LineCasting");
            string tag = hit.collider.tag;
            //Debug.Log("Name Collider: " + tag);
            IDamageAble damageable;
            switch (tag)
            {
                case "Player":
                    //Debug.Log("Hit Player");
                    damageable = hit.collider.GetComponentInParent<IDamageAble>();
                    damageable.TakeDamage(bulletDamage);
                    break;
                case "Enemy":
                    //Debug.Log("Hit Enemy");
                    BulletImpact(hit, ImpactFleshPrefab);

                    damageable = hit.collider.GetComponent<IDamageAble>();
                    damageable.TakeDamage(bulletDamage);

                    break;
                case "Enviroment":
                    //Debug.Log("Hit Enviroment");
                    BulletImpact(hit, ImpactEnviromentPrefab);
                    break;
            }
        }
    }

    private void BulletImpact(RaycastHit impact, GameObject impactPrefab) 
    {   

        GameObject impactObject = objectPool.GetObject(impactPrefab);
        impactObject.transform.position = impact.point;
        impactObject.transform.forward = impact.normal;

        ImpactFleshPrefab.SetActive(true);
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
}
