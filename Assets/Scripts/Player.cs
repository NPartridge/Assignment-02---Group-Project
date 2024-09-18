using UnityEngine;

// Information to help rotate to mouse position found here: https://discussions.unity.com/t/rotate-towards-mouse-position/883950
public class Player : MonoBehaviour
{
    [Header("Player stat sheet")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private int health = 100;

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            Debug.Log("Current player health: " + value);
            if (health <= 0)
            {
                Debug.Log("Player is dead!");
            }
        }
    }

    public bool AutoAimEnabled { get; private set; }
 

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        MovePlayer();
        RotatePlayer();
        CheckAutoAimToggle();
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
        if (AutoAimEnabled)
        {
            RotateTowardsNearestEnemy();
        }
        else
        {
            RotateTowardsMouse();
        }
    }

    private void RotateTowardsNearestEnemy()
    {
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            Vector3 enemyDirection = nearestEnemy.transform.position - transform.position;
            enemyDirection.y = 0; // Keep only the horizontal direction
            
            // Calculate the rotation needed to face the enemy
            Quaternion targetRotation = Quaternion.LookRotation(enemyDirection);

            // Rotates towards enemy
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void RotateTowardsMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Only interested in position of mouse on x,z axes, keep player rotation on y axis
            Vector3 targetPoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            Vector3 direction = targetPoint - transform.position;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void CheckAutoAimToggle()
    {
        // Allows the player to toggle auto-aim on/off when pressing 'R'. Halls of Torment has the same exact system
        if (Input.GetKeyDown(KeyCode.R))
        {
            AutoAimEnabled = !AutoAimEnabled;
            Debug.Log("Auto-aim is now " + (AutoAimEnabled ? "enabled" : "disabled"));
        }
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float minDistance = Mathf.Infinity; // https://docs.unity3d.com/ScriptReference/Mathf.Infinity.html
        
        // Go through all the enemies and find the closest one
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }
}
