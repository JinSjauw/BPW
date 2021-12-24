using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private AudioClip explosionSound;
    private ParticleSystem[] explosion;

    [SerializeField] private float fuseTimer;
    [SerializeField] private float explosionRadius;
    [SerializeField] private int damage;

    private void Start()
    {
        explosion = GetComponentsInChildren<ParticleSystem>(); 
    }

    private void OnEnable()
    {
        gameObject.GetComponent<MeshCollider>().enabled = true;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        StartCoroutine(FuseTimer());
    }

    private void playExplosion() 
    {
        foreach (ParticleSystem particle in explosion) 
        {
            particle.Play();
        }
    }
    private void stopExplosion()
    {
        foreach (ParticleSystem particle in explosion)
        {
            particle.Stop();
        }
    }

    private IEnumerator FuseTimer() 
    {
        yield return new WaitForSeconds(fuseTimer);
        playExplosion();
        AudioManager.PlaySoundOnce(explosionSound, transform.position);
        Explode();
        StartCoroutine(Timer());
    }

    private void Explode() 
    {   
        LayerMask damageLayer = LayerMask.GetMask("Player", "Enemy");
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, damageLayer);
     
        foreach(Collider hit in hits) 
        {   
            if(hit.GetComponent<IDamageAble>() == null) 
            {
                hit.GetComponentInParent<IDamageAble>().TakeDamage(damage);
            }
            else 
            {
                hit.GetComponent<IDamageAble>().TakeDamage(damage);
            }
        }
        Debug.Log("Boom");
    }

    private IEnumerator Timer() 
    {
        yield return new WaitForSeconds(2f);
        stopExplosion();
    }
}
