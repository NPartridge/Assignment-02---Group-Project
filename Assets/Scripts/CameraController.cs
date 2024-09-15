using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    [SerializeField] Vector3 cameraOffset = new(0, 12, -8);

    private void Start()
    {
        transform.position = cameraOffset;
        transform.rotation = Quaternion.Euler(60, 0, 0);
    }

    void Update()
    {
        transform.position = target.position + cameraOffset;
    }
}
