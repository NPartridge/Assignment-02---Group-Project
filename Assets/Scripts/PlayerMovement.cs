using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 5f;

    void Update()
    {
        float forwardInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector3 direction = new Vector3(horizontalInput, 0, forwardInput);
        Vector3 velocity = direction.normalized * speed;

        transform.Translate(velocity * Time.deltaTime);
    }
}
