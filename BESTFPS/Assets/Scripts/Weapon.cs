using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    [Header("Drop Physicis")]
    public float throwForce;
    public float throwExtraForce;
    public float rotationForce;

    [Header("Weapon Stats")]
    [SerializeField] private int muzzleVelocity;
    [SerializeField] private int rateOfFire;
    [SerializeField] private int ammoCapacity;
    [SerializeField] private float reloadSpeed;
    [SerializeField] private float hitForce;
    private enum FireModes {BoltAction, Semi, FullAuto};
    [SerializeField] private FireModes myFireMode;

    //Weapon States
    private enum WeaponState { Ready, Firing, Reloading, Empty }
    private WeaponState weaponState;
    private bool held;
    private bool allowedToFire = true;
    private int currentAmmo;

    [Header("Data")]
    public int weaponGfxLayer;
    public GameObject weaponGfx;
    public GameObject ColliderMesh;

    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private GameObject bulletPrefab;

    private Recoil weaponRecoil;
    private Animator animator;
    private ObjectPool objectPool;
    private Transform playerCamera;
    private Rigidbody weaponRB;
    private SkinnedMeshRenderer[] skinnedMeshes = new SkinnedMeshRenderer[2];

    private void Start()
    {
        animator = GetComponent<Animator>();
        objectPool = FindObjectOfType<ObjectPool>();
        weaponRB = gameObject.AddComponent<Rigidbody>();
        weaponRB.mass = .1f;
        skinnedMeshes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        weaponState = WeaponState.Ready;
        currentAmmo = ammoCapacity;
    }
    
    private void Update()
    {   
        Shoot();
    }

    public void UpdateState(int i) 
    {
        weaponState = (WeaponState)i;
    }

    public void Aim(bool state)
    {
        animator.SetBool("Aiming", state);
    }

    public void Reload()
    {
        if(weaponState != WeaponState.Reloading)
        {
            string reload = "Reload";

            if(weaponState == WeaponState.Empty) 
            {
                reload = "Reload2";
            }

            animator.SetTrigger(reload);
            StartCoroutine(ReloadWeapon());
            Debug.Log("Reloading");
        }
    }

    private void Shoot()
    {
        if (currentAmmo == 0)
        {
            weaponState = WeaponState.Empty;
        }
        //Firing mode for Full Auto
        if (weaponState == WeaponState.Firing && allowedToFire) 
        {
            Debug.Log("Shot");
            GameObject bulletObject = objectPool.GetObject(bulletPrefab);
            Bullet bullet = bulletObject.GetComponent<Bullet>();
            bullet.SetBullet(muzzleTransform);
            bullet.ShootBullet(muzzleTransform.up * muzzleVelocity);
            //ApplyRecoil
            weaponRecoil.ApplyRecoil();
            
            currentAmmo--;
            StartCoroutine(FiringCooldown());
            if (myFireMode == FireModes.Semi || myFireMode == FireModes.BoltAction)
            {
                animator.SetTrigger("Shooting");
                weaponState = WeaponState.Ready;
            }
            else 
            {
                animator.SetBool("Shooting", true);
            }
        }
        else if(weaponState != WeaponState.Firing)
        {
            animator.SetBool("Shooting", false);
        }
    }

    private IEnumerator FiringCooldown()
    {
        allowedToFire = false;
        yield return new WaitForSeconds(60f / rateOfFire);
        allowedToFire = true;
    }

    private IEnumerator ReloadWeapon()
    {
        weaponState = WeaponState.Reloading;

        yield return new WaitForSeconds(reloadSpeed);
        currentAmmo = ammoCapacity;
        weaponState = WeaponState.Ready;
    }

    public void PickUp(Transform weaponHolder, Transform cameraPlayer)
    {
        if(held) return;
        Destroy(weaponRB);
        transform.parent = weaponHolder;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        weaponGfx.layer = weaponGfxLayer;
        playerCamera = cameraPlayer;
        //Turn off MeshCollider for collision
        weaponRecoil = FindObjectOfType<Recoil>();
        setColliderMesh(false);
        held = true;
    }

    public void Drop(Transform playerCamera)
    {
        if(!held) return;
        weaponRB = gameObject.AddComponent<Rigidbody>();
        weaponRB.mass = 0.1f;
        Vector3 forward = playerCamera.forward;
        forward.y = 0f;
        weaponRB.velocity = forward * throwForce;
        weaponRB.velocity += Vector3.up * throwExtraForce;
        weaponRB.angularVelocity = Random.onUnitSphere * rotationForce;
        weaponGfx.layer = 0;
        weaponRecoil = null;
        //Turn on MeshCollider for collision
        setColliderMesh(true);
        
        transform.parent = null;
        held = false;
    }

    private void setColliderMesh(bool state)
    {
        ColliderMesh.GetComponent<MeshCollider>().enabled = state;
        ColliderMesh.GetComponent<MeshRenderer>().enabled = state;
        
        //Set the skinned meshes.
        foreach(SkinnedMeshRenderer skinnedMesh in skinnedMeshes)
        {
            skinnedMesh.enabled = !state;
        }
    }
}
