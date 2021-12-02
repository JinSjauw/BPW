using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyController : MonoBehaviour
{

    [Header("Functional Options")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool isSprinting = false;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canHeadbob = true;
    [SerializeField] private bool willSlideOnSlopes = true;

    [Header("Movement Parameters")]
    [SerializeField] private Vector3 directionVelocity;
    [SerializeField] private Vector3 directionSlopeVelocity;
    [SerializeField] private Vector3 wantedVelocity;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float currentSpeed = 0;
    [SerializeField] private float lastSpeed = 0;
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 10.0f;
    [SerializeField] private float crouchSpeed = 2.0f;
    [SerializeField] private float slopeSpeed = 8f;
    [SerializeField] private float acceleration = 10f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpPower = 8.0f;
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private bool shouldJump = true;

    [Header("Look Parameters")]
    [SerializeField, Range(.01f, .1f)] private float lookSpeedX = 0.05f;
    [SerializeField, Range(.01f, .1f)] private float lookSpeedY = 0.05f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;
    
    [Header("Camera Control")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orientation;

    [Header("Ground Detection")]
    [SerializeReference] LayerMask groundLayer;
    [SerializeField] float groundDistance = .4f;
    RaycastHit slopeHit;

    private Rigidbody rigbod;    
    private float playerHeight;
    private Vector2 mousePosition;
    private float rotationX, rotationY = 0;
    
    private bool isGrounded
    {
        get
        {
            if(Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundLayer))
            {
                return true;
            }
            return false;
        }
    }

    private bool onSlope
    {
        get
        {
            if  (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
            {
                if(slopeHit.normal != Vector3.up)
                {
                    Debug.Log("On Slope");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }

    void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        rigbod = GetComponent<Rigidbody>();
        playerHeight = GetComponentInChildren<CapsuleCollider>().height;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Move(InputAction.CallbackContext context) 
    {
            Vector2 movement = context.ReadValue<Vector2>();
            directionVelocity = new Vector3(movement.x, directionVelocity.y, movement.y);
            if(context.performed)
            {
                isMoving = true;
            }else
            {
                isMoving = false;
            }

    }

    public void Jump(InputAction.CallbackContext context) 
    {
        if(context.performed)
            shouldJump = true;
    }

    public void Sprinting(InputAction.CallbackContext context) 
    {
        if (context.started && canSprint) 
        {
            isSprinting = true;
        }

        if(context.canceled)
        {
            isSprinting = false;
        }
    }

    public void Fire(InputAction.CallbackContext context) 
    {
        
    }

    public void MouseLook(InputAction.CallbackContext context) 
    {
        mousePosition = context.ReadValue<Vector2>();

        rotationY += mousePosition.x * lookSpeedX;
        rotationX -= mousePosition.y * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);

        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.transform.rotation *= Quaternion.Euler(0, mousePosition.x * lookSpeedX, 0);
    }

    void HandleMove()
    {
        targetSpeed = (isSprinting ? sprintSpeed : walkSpeed); 
        
        if(directionVelocity == Vector3.zero)
        {
            targetSpeed = 0.0f;
        }
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, acceleration * Time.fixedDeltaTime);
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, sprintSpeed);

        if(isGrounded && !onSlope)
        {
            wantedVelocity = orientation.transform.TransformDirection(directionVelocity);
        }

        if(onSlope && isGrounded)
        {
            Debug.Log("Giving slopeVelocity");
            directionSlopeVelocity = Vector3.ProjectOnPlane(directionVelocity, slopeHit.normal);
            wantedVelocity = orientation.transform.TransformDirection(directionSlopeVelocity);
        }

        wantedVelocity = wantedVelocity * currentSpeed;
        Vector3 finalVelocity = Vector3.Lerp(rigbod.velocity, wantedVelocity, acceleration * Time.fixedDeltaTime);

        if(isGrounded && !shouldJump)
        {
            rigbod.velocity = new Vector3(finalVelocity.x, finalVelocity.y, finalVelocity.z);
            
        }else if(!isGrounded && shouldJump)
        {
            rigbod.velocity = new Vector3(finalVelocity.x, rigbod.velocity.y, finalVelocity.z);
        }

        // rigbod.velocity = new Vector3(wantedVelocity.x, rigbod.velocity.y, wantedVelocity.z);
    }

    void HandleJump() 
    {
        if (shouldJump && isGrounded) 
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
        if(!isGrounded)
        {
            rigbod.velocity -= new Vector3(0, gravity * Time.fixedDeltaTime, 0);
        }   
    }

    // bool isGrounded() 
    // {
    //     if(Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundLayer))
    //     {
    //         return true;
    //     }
    //     return false;
    // }



    void FixedUpdate() 
    {
        HandleMove();
        HandleJump();
        HandleGravity();   
    }
}
