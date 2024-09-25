using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float maxDistance = 20f;

    Vector3 startPosition;
    private int damage = 0;
    private bool isCritical = false;
    
    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        BulletMovement();
    }

    private void BulletMovement()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));

        // Destroy bullet when it goes beyond its max range
        Vector3 bulletDisplacement = transform.position - startPosition;
        float bulletDistance = bulletDisplacement.magnitude;

        if (bulletDistance > maxDistance)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetDamage(int damageAmount, bool critical)
    {
        damage = damageAmount;
        isCritical = critical;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Get the enemy the bullet collided with
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, isCritical); // Deal the damage
            }

            // Destroy the bullet on hit
            Destroy(gameObject);
        }
    }
}
