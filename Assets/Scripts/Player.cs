using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player stat sheet")]
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
