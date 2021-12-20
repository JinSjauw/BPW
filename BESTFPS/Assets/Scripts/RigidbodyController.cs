using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyController : MonoBehaviour
{
    [SerializeField] private bool isSprinting = false;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canHeadbob = true;
    [SerializeField] private bool willSlideOnSlopes = true;

    [Header("Movement Parameters")]
    [SerializeField] private float targetSpeed;
    [SerializeField] private float currentSpeed = 0;
    [SerializeField] private float lastSpeed = 0;
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 10.0f;
    [SerializeField] private float crouchSpeed = 2.0f;
    [SerializeField] private float slopeSpeed = 8f;
    [SerializeField] private float acceleration = 3.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpPower = 8.0f;
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private bool shouldJump = true;

    [SerializeField] private Vector3 directionVelocity, currentVelocity, wantedVelocity;
    //[SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orientation;

    private Rigidbody rigbod;
    
    private void Awake()
    {
        //playerCamera = GetComponentInChildren<Camera>();
        rigbod = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnMove(InputValue context) 
    {
            Vector2 movement = context.Get<Vector2>();
            directionVelocity = new Vector3(movement.x, directionVelocity.y, movement.y).normalized;
    }

    private void OnJump(InputValue context) 
    {
        Debug.Log("Detected Jump Input");
        if(context.isPressed)
            shouldJump = true;
    }

    private void OnSprint(InputValue context) 
    {
        Debug.Log(context.isPressed);
        if (context.isPressed && canSprint) 
        {
            isSprinting = true;
        }
        if(!context.isPressed)
        {
            isSprinting = false;
        }
    }

    void HandleMove()
    {
        targetSpeed = (isSprinting ? sprintSpeed : walkSpeed);
        wantedVelocity = orientation.transform.TransformDirection(directionVelocity);
        
        if(directionVelocity == Vector3.zero)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, acceleration * Time.fixedDeltaTime) ;
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        }
        currentSpeed = Mathf.Clamp(currentSpeed, 0, sprintSpeed);
        wantedVelocity = wantedVelocity * currentSpeed;

        rigbod.velocity = new Vector3(wantedVelocity.x, rigbod.velocity.y, wantedVelocity.z);
    }

    void HandleJump() 
    {
        if (shouldJump && isGrounded()) 
        {
            Debug.Log("Jumping");
            rigbod.velocity += new Vector3(0, jumpPower, 0);        
        }else
        {
            shouldJump = false;
        }
    }

    void HandleGravity() 
    {
        if(!isGrounded())
        {
            rigbod.velocity -= new Vector3(0, gravity * Time.fixedDeltaTime, 0);
        }   
    }

    bool isGrounded() 
    {
        LayerMask groundLayer = LayerMask.GetMask("Enviroment");
        Debug.DrawRay(transform.position, Vector3.down, Color.blue);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f, groundLayer))
        {
            return true;
        }
        return false;
    }

    void FixedUpdate() 
    {
        HandleMove();
        HandleJump();
        HandleGravity();   
    }
}
