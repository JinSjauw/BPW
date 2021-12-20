using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Hip Recoil")]
    [SerializeField] private float recoilX;
    [SerializeField] private float recoilY;
    [SerializeField] private float recoilZ;
    
    [Header("ADS Recoil")]
    [SerializeField] private float adsRecoilX;
    [SerializeField] private float adsRecoilY;
    [SerializeField] private float adsRecoilZ;

    [SerializeField] private float snapForce;
    [SerializeField] private float returnSpeed;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapForce * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
        Debug.Log("Transform: " + transform.localRotation);
        Debug.Log("Recoil being applied: " + targetRotation);
    }

    public void ApplyRecoil()
    {   
        targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        //Debug.Log("Recoil being applied: " + targetRotation);
    }
}
