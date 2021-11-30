using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyController : MonoBehaviour
{
    private bool isSprinting = false;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canHeadbob = true;
    [SerializeField] private bool willSlideOnSlopes = true;

    [Header("Movement Parameters")]
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
    [SerializeField, Range(.1f, 10f)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(.1f, 10f)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [SerializeField] private Vector3 directionVelocity, wantedVelocity;

    private Rigidbody rigbod;
    private Camera playerCamera;
    private Vector2 mousePosition;

    private float currentSpeed = 0;
    private float rotationX = 0;

    // Start is called before the first frame update
    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        rigbod = GetComponent<Rigidbody>();
    }

    public void Move(InputAction.CallbackContext context) 
    {
        Vector2 movement = context.ReadValue<Vector2>();
        directionVelocity = new Vector3(movement.x, directionVelocity.y, movement.y);
        //Debug.Log("Movement Direction: " + directionVelocity);
    }

    public void Jump(InputAction.CallbackContext context) 
    {
        if (context.performed && isGrounded()) 
        {
            Debug.Log("Jumped");
            shouldJump = true;
        }
    }

    public void Sprinting(InputAction.CallbackContext context) 
    {
        if (context.started && canSprint) 
        {
            isSprinting = true;
        }
    }

    public void Fire(InputAction.CallbackContext context) 
    {
        
    }

    public void MouseLook(InputAction.CallbackContext context) 
    {
        mousePosition = context.ReadValue<Vector2>();
    }

    void HandleMouse() 
    {
        rotationX -= mousePosition.y * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mousePosition.x * lookSpeedX, 0);
    }

    void HandleMove()
    {
        float moveSpeed = (isSprinting ? sprintSpeed : walkSpeed);

        if (directionVelocity == new Vector3(0, 0, 0))
        {
            currentSpeed = currentSpeed - deceleration * Time.fixedDeltaTime;
        }
        else 
        {
            wantedVelocity = directionVelocity;
            currentSpeed = currentSpeed + acceleration * Time.fixedDeltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, moveSpeed);
        rigbod.velocity = transform.TransformDirection(wantedVelocity) * currentSpeed;
    }

    void HandleJump() 
    {
        if (shouldJump) 
        {
            Debug.Log("Jumping");
            directionVelocity.y = jumpPower;
        }
    }

    void HandleGravity() 
    {
        if (!isGrounded())
        {
            Debug.Log("Gravity Applied");
            directionVelocity.y -= gravity * Time.fixedDeltaTime;
        }
        else 
        {
            directionVelocity.y = 0f;
        }
    }

    bool isGrounded() 
    {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        Debug.DrawRay(transform.position, Vector3.down, Color.blue);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f, groundLayer))
        {
            return true;
        }
        shouldJump = false;
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouse();
    }

    void FixedUpdate() 
    {
        HandleGravity();
        HandleJump();
        HandleMove();
    }
}
