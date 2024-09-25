using UnityEngine;

// Information to help rotate to mouse position found here: https://discussions.unity.com/t/rotate-towards-mouse-position/883950
public class Player : MonoBehaviour
{
    [Header("Player stat sheet")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private int currentHealth;
    [SerializeField] private int baseMaximumHealth = 100;
    private int flatHealthIncrease = 0;

    public int experience = 0;
    public float pickupRadius = 5f;

    [SerializeField] private float critChance = 0f;
    [SerializeField] private float critDamageMultiplier = 2f;

    [SerializeField] private float attackSpeed = 1f;

    [SerializeField] private int basePlayerDamage = 0;
    private int flatPlayerDamageIncrease = 0;

    // We need attack speed, crit chance, and crit multi in the weapon script
    public float AttackSpeed => attackSpeed;
    public float CritChance => critChance;
    public float CritDamageMultiplier => critDamageMultiplier;

    [Header("Levelling System")]
    public int level = 1;
    public int experienceToNextLevel = 0;

    public int MaximumHealth
    {
        get => baseMaximumHealth + flatHealthIncrease;
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, MaximumHealth);
            Debug.Log("Current player health: " + currentHealth + "/" + MaximumHealth);
            if (currentHealth <= 0)
            {
                // Tell the animator that the player is dead, and perform death animation
                animator.SetBool("isDead", true);
                animator.SetTrigger("die");
                Debug.Log("Player is dead!");
            }
        }
    }

    public bool AutoAimEnabled { get; private set; }

    private Animator animator;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        CurrentHealth = MaximumHealth; // Init current health to max HP on start
        animator = GetComponent<Animator>();
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

        // Update Animation Parameters
        animator.SetFloat("velocityX", direction.x);
        animator.SetFloat("velocityZ", direction.z);

        transform.Translate(velocity * Time.deltaTime, Space.Self);
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
            Enemy enemyScript = enemy.GetComponent<Enemy>();

            // We want to avoid targeting enemies that are dead
            if (enemyScript == null || enemyScript.IsDead)
                continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        //Debug.Log("Player received: " + amount + " experience. Total player experience: " + experience);
    }

    void OnDrawGizmosSelected()
    {
        // A yellow sphere of the players pickup radius. The player needs to be selected in the editor for this to appear
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
