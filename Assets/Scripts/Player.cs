using UnityEngine;

// Information to help rotate to mouse position found here: https://discussions.unity.com/t/rotate-towards-mouse-position/883950
public class Player : MonoBehaviour
{
    [Header("Player stat sheet")]
    [SerializeField] float speed = 5f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        RotatePlayer();
        MovePlayer();
    }

    private void MovePlayer()
    {
        float forwardInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 direction = new Vector3(horizontalInput, 0, forwardInput).normalized;
        Vector3 velocity = direction * speed;

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private void RotatePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Only interested in position of mouse on x,z axes, keep player rotation on y axis
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
