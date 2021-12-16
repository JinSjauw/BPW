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
    [SerializeField] private int ammoCapacity;
    [SerializeField] private int rateOfFire;
    [SerializeField] private float reloadSpeed;
    [SerializeField] private float hitForce;
    private enum FireModes {BoltAction, Semi, FullAuto};
    [SerializeField] private FireModes myFireMode;

    [Header("Data")]
    public int weaponGfxLayer;
    public GameObject weaponGfx;
    public GameObject ColliderMesh;

    private bool held;
    private bool reloading;
    private bool shooting;
    private bool totalAmmo;
    private Transform playerCamera;
    private Rigidbody weaponRB;
    private SkinnedMeshRenderer[] skinnedMeshes = new SkinnedMeshRenderer[2];


    private void Start()
    {   
        weaponRB = gameObject.AddComponent<Rigidbody>();
        weaponRB.mass = .1f;
        skinnedMeshes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

    }

    public void OnFire(InputValue context)
    {
        //bool triggerMode = (myFireMode == FireModes.BoltAction || myFireMode == FireModes.Semi && !shooting && !reloading ? context.isPressed : !context.isPressed);
        if(context.isPressed)
        {
        Debug.Log("Fire Mode: " + myFireMode + " Context:" + context);
        }

    }

    public void OnReload(InputValue context)
    {
        if(context.isPressed && !reloading)
        {
            StartCoroutine(Reload());
            Debug.Log("Reloading");
        }
    }

    private void Shoot()
    {
        //Shoot Projectile
    }

    private IEnumerator FiringCooldown()
    {
        shooting = true;
        yield return new WaitForSeconds(1f / rateOfFire);
        shooting = false;
    }

    private IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadSpeed);
        reloading = false;
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
