using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RigidbodyController : MonoBehaviour
{

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
    [SerializeField] private bool isGroundedBool = true;
    [Header("Look Parameters")]
    [SerializeField, Range(.1f, 10f)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(.1f, 10f)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    private Rigidbody rigbod;
    private Camera playerCamera;
    [SerializeField] private Vector3 movementDirection, wantedVelocity;
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
        movementDirection = new Vector3(movement.x, movementDirection.y, movement.y);
        //Debug.Log("Movement Direction: " + movementDirection);
    }

    public void Jump(InputAction.CallbackContext context) 
    {
        if (context.performed && isGrounded()) 
        {
            Debug.Log("Jumped");
            movementDirection.y = jumpPower;
        }

    }

    public void Fire(InputAction.CallbackContext context) 
    {
        
    }

    public void MouseLook(InputAction.CallbackContext context) 
    {
        mousePosition = context.ReadValue<Vector2>();
        //Debug.Log("Mouse Coords: " + context.ReadValue<Vector2>());
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
        if (movementDirection == new Vector3(0, 0, 0))
        {
            currentSpeed = currentSpeed - deceleration * Time.fixedDeltaTime;
        }
        else 
        {
            wantedVelocity = movementDirection;
            currentSpeed = currentSpeed + acceleration * Time.fixedDeltaTime;
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0, walkSpeed);
        rigbod.velocity = transform.TransformDirection(wantedVelocity) * currentSpeed;
    }

    void HandleGravity() 
    {
        if (!isGrounded())
        {
            movementDirection.y -= gravity * Time.fixedDeltaTime;
        }
    }

    bool isGrounded() 
    {
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        Debug.DrawRay(transform.position, Vector3.down, Color.blue);
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.4f, groundLayer))
        {
            isGroundedBool = true;
            return isGroundedBool;
        }
        movementDirection.y = 0.0f;
        isGroundedBool = false;
        return isGroundedBool;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouse();
    }

    void FixedUpdate() 
    {
        HandleMove();
        HandleGravity();
    }



}
