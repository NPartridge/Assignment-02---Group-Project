using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float maxDistance = 20f;
    void Start()
    {

    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        // Destroy bullet when it goes beyond its max range
        float distanceTravelled = transform.position.magnitude;
        if (distanceTravelled > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Destroy the enemy on hit
            Destroy(other.gameObject);
            // Destroy the bullet on hit
            Destroy(gameObject);

        }
    }
}
