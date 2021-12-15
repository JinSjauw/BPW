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
    private Rigidbody weaponRB;
    private SkinnedMeshRenderer[] skinnedMeshes = new SkinnedMeshRenderer[2];


    private void Start()
    {   
        weaponRB = gameObject.AddComponent<Rigidbody>();
        weaponRB.mass = .1f;
        skinnedMeshes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

    }

    public void Fire(InputAction.CallbackContext context)
    {
        bool triggerMode = (myFireMode == FireModes.BoltAction || myFireMode == FireModes.Semi ? context.performed : context.started);
        Debug.Log("Fire Mode: " + myFireMode + " Context:" + context);
    }

    public void PickUp(Transform weaponHolder)
    {
        if(held) return;
        Destroy(weaponRB);
        transform.parent = weaponHolder;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        weaponGfx.layer = weaponGfxLayer;
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
