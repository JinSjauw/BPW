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
    [SerializeField] private bool canCrouch = true;    
    [SerializeField] private bool canHeadbob = true;
    [SerializeField] private bool willSlideOnSlopes = true;

    [Header("Movement Parameters")]
    [SerializeField] private Vector3 directionVelocity;
    [SerializeField] private Vector3 directionSlopeVelocity;
    [SerializeField] private Vector3 wantedVelocity;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float walkSpeed = 5.0f;
    [SerializeField] private float sprintSpeed = 10.0f;
    
    [SerializeField] private float slopeSpeed = 8f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float gravity = 30.0f;
    
    [Header("Crouch Parameters")]
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool inCrouchTransition;
    [SerializeField] private float crouchSpeed = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);

    [Header("Sliding Parameters")]
    [SerializeField] private bool isSliding = false;
    [SerializeField] private float slideSpeed = 8f;

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

    private Rigidbody playerBody;    
    private CapsuleCollider playerCollider;
    private Vector2 mousePosition;
    private float rotationX, rotationY = 0;
    
    private bool isGrounded
    {
        get
        {
            if(Physics.CheckSphere(transform.position - new Vector3(0, playerCollider.height/2, 0), groundDistance, groundLayer))
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
            if  (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerCollider.height / 2 + 0.5f))
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
        playerBody = GetComponent<Rigidbody>();
        playerCollider = GetComponentInChildren<CapsuleCollider>();
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

    public void Crouch(InputAction.CallbackContext context)
    {
        if(context.performed && !isSprinting)
        {
            StartCoroutine(CrouchStand());
        }else if(context.performed && isSprinting)
        {
            isSliding = true;
            StartCoroutine(CrouchStand());
        }
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
        targetSpeed = (isSliding? slideSpeed : isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed); 
        
        wantedVelocity = orientation.transform.TransformDirection(directionVelocity.normalized) * targetSpeed;
        playerBody.velocity = Vector3.Lerp(playerBody.velocity, wantedVelocity, acceleration * Time.fixedDeltaTime);

    }
    
     private IEnumerator CrouchStand()
    {
        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        inCrouchTransition = true;

        float timeElapsed = 0f;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = playerCollider.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = playerCollider.center;

        while(timeElapsed < timeToCrouch)
        {
            playerCollider.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            playerCollider.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCollider.height = targetHeight;
        playerCollider.center = targetCenter;

        isCrouching = !isCrouching;

        inCrouchTransition = false;
    }

    void HandleGravity() 
    {
        if(!isGrounded)
        {
            playerBody.AddForce(Vector3.down * gravity * 2, ForceMode.Acceleration);
        } else {
            wantedVelocity.y = 0;
        }
    }
    void FixedUpdate() 
    {
        HandleMove();
        HandleGravity();   
    }
}
