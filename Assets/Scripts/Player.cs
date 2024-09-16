using UnityEngine;

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
        float forwardInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 direction = new Vector3(horizontalInput, 0, forwardInput).normalized;
        Vector3 velocity = direction * speed;

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private void RotatePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }
    }
}
