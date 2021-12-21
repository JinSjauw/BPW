using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private float smooth;
    [SerializeField] private float swayMultiplayer;

    private MouseLook mouseLook;
    private Vector2 mousePos;
    
    private void Awake()
    {
        mouseLook = FindObjectOfType<MouseLook>();
    }

    private void Update()
    {
        mousePos = mouseLook.GetMousePos();
        mousePos *= swayMultiplayer;

        Quaternion rotationX = Quaternion.AngleAxis(-mousePos.y, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mousePos.x, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.fixedDeltaTime);
    }
}
