using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

public bool CanMove { get; private set; } = true;
private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !inTransition && characterController.isGrounded;

[Header("Functional Options")]
[SerializeField] private bool canSprint = true;
[SerializeField] private bool canJump = true;
[SerializeField] private bool canCrouch = true;
[SerializeField] private bool canHeadbob = true;
[SerializeField] private bool willSlideOnSlopes = true;

[Header("Controls")]
[SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
[SerializeField] private KeyCode jumpKey = KeyCode.Space;
[SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

[Header("Movement Parameters")]
[SerializeField] private float walkSpeed = 5.0f;
[SerializeField] private float sprintSpeed = 10.0f;
[SerializeField] private float crouchSpeed = 2.0f;
[SerializeField] private float slopeSpeed = 8f;

[Header("Look Parameters")]
[SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
[SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
[SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
[SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

[Header("Jumping Parameters")]
[SerializeField] private float jumpPower = 8.0f;
[SerializeField] private float gravity = 30.0f;

[Header("Crouch Parameters")]
[SerializeField] private float crouchHeight = 0.5f;
[SerializeField] private float standingHeight = 2.0f;
[SerializeField] private float timeToCrouch = 0.25f;
[SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
[SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
private bool isCrouching;
private bool inTransition;

[Header("Headbob Parameters")]
[SerializeField] private float walkBobSpeed = 14f;
[SerializeField] private float walkBobAmount = 0.05f;
[SerializeField] private float sprintBobSpeed = 18f;
[SerializeField] private float sprintBobAmount = 0.1f;
[SerializeField] private float crouchBobSpeed = 7f;
[SerializeField] private float crouchBobAmount = 0.025f;

private float defaultYPos = 0;
private float timer;

//Sliding Parameters

private Vector3 hitPointNormal;
private bool isSliding 
{
    get
    {
        Debug.DrawRay(transform.position, Vector3.down, Color.red);
        if(characterController.isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 3f))
        {
            hitPointNormal = slopeHit.normal;
            return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
        }
        else
        {
            return false;
        }
    }
}

private Camera playerCamera;
private CharacterController characterController;

private Vector3 moveDirection;
private Vector2 currentInput;

private float rotationX = 0;


void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove) 
        {
            HandleMovementInput();
            HandleMouseLook();
            
            if(canHeadbob)
                HandleHeadBob();

            if(canJump && !isSliding)
                HandleJump();

            if(canCrouch)
                HandleCrouch();

            ApplyFinalMovement();
        }
    }

    private void HandleMovementInput() 
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook() 
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        if(ShouldJump)
            moveDirection.y = jumpPower;
    }

    private void HandleCrouch()
    {
        if(ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void HandleHeadBob()
    {
        if(!characterController.isGrounded) return;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x, 
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void ApplyFinalMovement() 
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        
        if(willSlideOnSlopes && isSliding)
        {
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        inTransition = true;

        float timeElapsed = 0f;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        inTransition = false;
    }


}
