using UnityEngine;

public class Recoil : MonoBehaviour
{

    [SerializeField] private Vector3 recoil;

    [SerializeField] private float snapForce;
    [SerializeField] private float returnSpeed;

    private Vector3 currentRotation;
    private Vector3 targetRotation;

    void Update()
    {
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, snapForce * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    public void SetRecoil(Vector3 recoilWeapon) 
    {
        recoil = recoilWeapon;
    }
    public void ApplyRecoil()
    {
        targetRotation += new Vector3(-recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z));
        //Debug.Log("Recoil being applied: " + targetRotation);
    }
}
