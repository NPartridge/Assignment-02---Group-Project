using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
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
            // In the case of a bullet spray, destroy the parent container of the bullets.
            if(transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
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
        
        if (other.CompareTag("Player"))
        {
            
            // Get the enemy the bullet collided with
            Player player = other.gameObject.GetComponent<Player>();

            if (player != null)
            {
                
                player.CurrentHealth -= damage;
            }

            // Destroy the bullet on hit
            Destroy(gameObject);
        }
    }
}