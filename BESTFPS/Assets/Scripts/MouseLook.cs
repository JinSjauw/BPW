using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{

    [Header("Look Parameters")]
    [SerializeField, Range(.01f, .1f)] private float lookSpeedX = 0.05f;
    [SerializeField, Range(.01f, .1f)] private float lookSpeedY = 0.05f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerCamera;

    private Vector2 mousePosition;
    public float rotationY, rotationX;
    private void OnLook(InputValue context)
    {
        mousePosition = context.Get<Vector2>();

        rotationY += mousePosition.x * lookSpeedX;
        rotationX -= mousePosition.y * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);

        playerCamera.transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
        orientation.transform.rotation *= Quaternion.Euler(0, mousePosition.x * lookSpeedX, 0);
    }

    public Vector2 GetMousePos()
    {
        return mousePosition;
    }
}
