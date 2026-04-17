using UnityEngine;

public class FlashlightManager : MonoBehaviour
{
    [SerializeField]
    private Transform playerCamera;

    // Update is called once per frame
    void Update()
    {
        TraceCamera();
    }

    void TraceCamera()
    {
        this.transform.position = playerCamera.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, playerCamera.rotation, 0.05f);
    }
}
