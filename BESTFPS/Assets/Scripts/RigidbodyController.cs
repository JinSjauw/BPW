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
    [SerializeField] private float deceleration = 3.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpPower = 8.0f;
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private bool shouldJump = true;

    [Header("Look Parameters")]
    [SerializeField, Range(.01f, .1f)] private float lookSpeedX = 0.05f;
    [SerializeField, Range(.01f, .1f)] private float lookSpeedY = 0.05f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [SerializeField] private Vector3 directionVelocity, wantedVelocity;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orientation;

    private Rigidbody rigbod;
    
    private Vector2 mousePosition;
    private float rotationX, rotationY = 0;

    // Start is called before the first frame update
    void Awake()
    {
        //playerCamera = GetComponentInChildren<Camera>();
        rigbod = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Move(InputAction.CallbackContext context) 
    {
            Vector2 movement = context.ReadValue<Vector2>();
            directionVelocity = new Vector3(movement.x, directionVelocity.y, movement.y);
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
        LayerMask groundLayer = LayerMask.GetMask("Ground");
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
