using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCam.transform.forward);
    }
}
