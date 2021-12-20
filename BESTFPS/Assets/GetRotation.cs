using UnityEngine;
using UnityEngine.InputSystem;


public class GetRotation : MonoBehaviour
{

    [SerializeField] private Transform rotation;

    private void Update()
    {
        transform.position = rotation.position;
     
    }
}
