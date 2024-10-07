
using System;
using System.Collections;

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
    //private int flatPlayerDamageIncrease = 0;

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
    
    public event Action<int, int> OnHealthChanged;

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            int previousHealth = currentHealth;
            currentHealth = Mathf.Clamp(value, 0, MaximumHealth);
            
            if (currentHealth != previousHealth)
            {
                OnHealthChanged?.Invoke(currentHealth, MaximumHealth);
                //Debug.Log("Current player health: " + currentHealth + "/" + MaximumHealth);
            }

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
 
    private UpgradeManager upgradeManager;

    private Animator animator;

    private CharacterController characterController;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        upgradeManager = FindObjectOfType<UpgradeManager>();
        if (upgradeManager == null)
        {
            Debug.LogError("No upgrade manager in scene");
        }
        CurrentHealth = MaximumHealth; // Init current health to max HP on start

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
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


        // We want to move the player in world space, but we also want the character model to always face an enemy target when
        // auto-attacking or the mouse position when not 
        // Further, if the character is facing to the right, pressing the horizontal input key for right, should walk it forward
        // and pressing the horizontal left input should walk it backward
        // Similarly, if the character is facing forward, pressing the vertical input key should walk it forward, and vertical input key
        // for back should walk it backward
        float forwardDotProduct = Vector3.Dot(direction, transform.forward);
        float horizontalDotProduct = Vector3.Dot(direction, transform.right);

        // Update Animation Parameters
        animator.SetFloat("velocityX", horizontalDotProduct);
        animator.SetFloat("velocityZ", forwardDotProduct);

        Vector3 velocity = direction * speed;

        characterController.Move(velocity * Time.deltaTime);
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
        CheckLevelUp();
    }
    
    // Currently levelling up is based on a quadratic formula (the change in the change of our y is constant)
    // Below is the output for the current formula if the starting experience is 100
    // Base exp = 100 -> 400 -> 900 -> 1600 -> 2500 -> 3600 -> (This is the change in our y, NOT CONSTANT)
    // Differences =  300 -> 500 -> 700 -> 900 -> 1100 -> (This is the change in the change of our y, IS CONSTANT)
    // We probably do not want a linear or exponential system for this game because the player should be able
    // to level at a steady pace without exp requirements becoming either too steep or too easy
    private void CheckLevelUp()
    {
        // This loop is to help handle multiple level-ups when the experience is greater than one levelup threshold
        while (experience >= CalculateTotalExperienceForLevel(level + 1))
        {
            level++;
            Debug.Log("Player leveled up to level " + level + "!");
            
            if (upgradeManager != null)
            {
                upgradeManager.ShowUpgradeOptions(this);
            }
        }
        
        experienceToNextLevel = CalculateTotalExperienceForLevel(level + 1);
    }

    private int CalculateTotalExperienceForLevel(int targetLevel)
    {
        int baseXP = 100;
        return baseXP * (targetLevel - 1) * (targetLevel - 1);
    }
    
    public void UpgradeMovementSpeed(float amount)
    {
        speed += amount;
        Debug.Log("Movement speed increased to " + speed);
    }

    public void UpgradeRotationSpeed(float amount)
    {
        rotationSpeed += amount;
        Debug.Log("Rotation speed increased to " + rotationSpeed);
    }
    
    public void UpgradeFlatHealth(int amount)
    {
        flatHealthIncrease += amount;
        Debug.Log("Flat health increased by " + amount + ". Total health is now " + MaximumHealth);
        
        // Increase the current HP as well as the maximum HP
        CurrentHealth += amount;
    }

    public void UpgradePercentHealth(float amount)
    {
        
    }

    public void UpgradePickupRadius(float amount)
    {
        pickupRadius += amount;
        Debug.Log("Pickup radius increased to " + pickupRadius);
    }
    
    public void UpgradeCritChance(float percentage)
    {
        critChance += percentage / 100f;
        Debug.Log("Crit chance increased to " + critChance * 100f + "%");
    }
    
    public void UpgradeCritDamage(float multiplierIncrease)
    {
        critDamageMultiplier += multiplierIncrease;
        Debug.Log("Crit damage multi increased to " + critDamageMultiplier + "x");
    }
    
    public void UpgradeAttackSpeed(float amount)
    {
        attackSpeed += amount;
        Debug.Log("Attack speed increased to " + attackSpeed);
    }
    
    public void UpgradeDamage(int amount)
    {
        basePlayerDamage += amount;
        Debug.Log("Player damage increased by " + amount + ". Total player damage is now " + TotalPlayerDamage);
    }
    
    public int TotalPlayerDamage
    {
        get => basePlayerDamage;
    }

    public IEnumerator ApplyTemporarySpeedBuff(float amount, float duration)
    {
        // store the unmodified value
        float originalValue = speed;
        // increase the value by the amount
        speed += amount;
        // wait for the duration
        yield return new WaitForSecondsRealtime(duration);
        // revert to the original value
        speed = originalValue;
    }
    
    void OnDrawGizmosSelected()
    {
        // A yellow sphere of the players pickup radius. The player needs to be selected in the editor for this to appear
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
