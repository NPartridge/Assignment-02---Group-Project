using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Information to help rotate to mouse position found here: https://discussions.unity.com/t/rotate-towards-mouse-position/883950
public class Player : MonoBehaviour
{
    [SerializeField] SoundEffect soundEffect;

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
    [SerializeField] private float range = 20f;
    //private int flatPlayerDamageIncrease = 0;

    // We need attack speed, crit chance, and crit multi, and a check for if the player is alive in the weapon script
    public float AttackSpeed => attackSpeed;
    public float CritChance => critChance;
    public float CritDamageMultiplier => critDamageMultiplier;
    public bool IsDead => isDead;

    [Header("Levelling System")]
    public int level = 1;
    public int TotalExperienceForCurrentLevel => CalculateTotalExperienceForLevel(level);
    public int TotalExperienceForNextLevel => CalculateTotalExperienceForLevel(level + 1);
    public int ExperienceRequiredForNextLevel => TotalExperienceForNextLevel - TotalExperienceForCurrentLevel;
    public int ExperienceIntoCurrentLevel => experience - TotalExperienceForCurrentLevel;
    public event Action OnExperienceChanged;

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
                // Play player death sound FX
                audioSource.PlayOneShot(soundEffect.PlayerDie);

                // Tell the animator that the player is dead, show death animation, display game over screen
                animator.SetBool("isDead", true);
                animator.SetTrigger("die");
                _gameOverManager.ShowGameOverUI();
                isDead = true;
                //Debug.Log("Player is dead!");
            }
        }
    }
    
    public event Action OnSpeedBuffStarted;
    public event Action OnSpeedBuffEnded;
    
    private Coroutine speedBuffCoroutine;
    private float originalSpeed;

    public bool AutoAimEnabled { get; private set; }

    private UpgradeManager upgradeManager;
    private GameOverManager _gameOverManager;
    private PauseManager pauseManager;
    private Animator animator;
    private bool isDead = false;

    private AudioSource audioSource;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        upgradeManager = FindObjectOfType<UpgradeManager>();
        _gameOverManager = FindObjectOfType<GameOverManager>();
        pauseManager = FindObjectOfType<PauseManager>();

        if (upgradeManager == null || _gameOverManager == null || pauseManager == null)
        {
            Debug.LogError("No upgrade manager, pause manager, and or game over manager in scene");
        }
        
        originalSpeed = speed;
        CurrentHealth = MaximumHealth; // Init current health to max HP on start
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // We don't want to update the player if they are dead
        if (!isDead)
        {
            // We don't want to update the player if there is a menu open
            if (!pauseManager.IsAnyMenuOpen())
            {
                MovePlayer();
                RotatePlayer();
                CheckAutoAimToggle();
            }
        }
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

        transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private void RotatePlayer()
    {
        if (AutoAimEnabled)
        {
            RotateTowardsNearestTarget();
        }
        else
        {
            RotateTowardsMouse();
        }
    }

    private void RotateTowardsNearestTarget()
    {
        GameObject nearestTarget = FindNearestTarget();

        if (nearestTarget != null)
        {
            Vector3 targetDirection = nearestTarget.transform.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void RotateTowardsMouse()
    {
        // Project a ray from the camera to the mouse positon
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Rotate the player transform around its vertical axis to face the (x, z) co-ordinates of the mouse cursor in world space
            Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.LookAt(targetPosition);
            
            // Get  player's firing point and its direction to the target
            Transform firingPoint = transform.Find("WeaponFiringPoint");
            Vector3 targetDirection = (hit.point - firingPoint.position);

            // Only allow the player to shoot if it a minimum distance from target.
            if(targetDirection.magnitude > 3f)
            {
                // Rotate the firing point to face the target
                Quaternion rotationToTarget = Quaternion.LookRotation(targetDirection);
                firingPoint.rotation = Quaternion.Slerp(firingPoint.rotation, rotationToTarget, Time.deltaTime * rotationSpeed);


                // Get the gun model and its direction to the target
                GameObject gunModel = GameObject.FindGameObjectWithTag("Gun");
                Vector3 gunToTargetDirection = (hit.point - gunModel.transform.position).normalized;

                // Rotate the gun model to face the target
                Quaternion gunRotationToTarget = Quaternion.LookRotation(gunToTargetDirection);
                Quaternion.Slerp(gunModel.transform.rotation, gunRotationToTarget, Time.deltaTime * rotationSpeed);
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

    private GameObject currentTarget;

    private GameObject FindNearestTarget()
    {
        // Our current targets are enemies and barrels
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] crates = GameObject.FindGameObjectsWithTag("Destructible");

        List<GameObject> potentialTargets = new List<GameObject>();
        potentialTargets.AddRange(enemies);
        potentialTargets.AddRange(crates);

        GameObject nearestTarget = null;
        float minDistance = Mathf.Infinity; // https://docs.unity3d.com/ScriptReference/Mathf.Infinity.html

        foreach (GameObject target in potentialTargets)
        {
            Enemy enemyScript = target.GetComponent<Enemy>();
            DestructibleObject destructibleScript = target.GetComponent<DestructibleObject>();

            // For enemies, we want to avoid targeting enemies that are dead
            if (enemyScript != null && enemyScript.IsDead)
                continue;

            if (destructibleScript != null && !destructibleScript.IsActive)
                continue;

            float distance = Vector3.Distance(transform.position, target.transform.position);

            // For destructibles, we only want to shoot and destructibles that are within the range of the player
            if (destructibleScript != null)
            {
                if (distance > range)
                    continue;
            }

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = target;
            }
        }

        currentTarget = nearestTarget;
        return nearestTarget;
    }

    // We need this in the weapon script for aiming at targets
    public GameObject CurrentTarget => currentTarget;

    public void AddExperience(int amount)
    {
        audioSource.PlayOneShot(soundEffect.ExperienceGemCollect);
        experience += amount;
        //Debug.Log("Player received: " + amount + " experience. Total player experience: " + experience);
        CheckLevelUp();
        OnExperienceChanged?.Invoke();
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
            // Play level up sound effect with slight delay to allow for xp gem sound effects
            audioSource.clip = soundEffect.LevelUp;
            audioSource.PlayDelayed(0.3f);

            level++;
            Debug.Log("Player leveled up to level " + level + "!");

            if (upgradeManager != null)
            {
                upgradeManager.ShowUpgradeOptions(this);
            }
        }
        
        OnExperienceChanged?.Invoke();
    }

    public int CalculateTotalExperienceForLevel(int targetLevel)
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

    public void ApplyTemporarySpeedBuff(float amount, float duration)
    {
        if (speedBuffCoroutine != null)
        {
            StopCoroutine(speedBuffCoroutine);
        }
        speedBuffCoroutine = StartCoroutine(SpeedBuffCoroutine(amount, duration));
    }

    private IEnumerator SpeedBuffCoroutine(float amount, float duration)
    {
        // Apply the speed buff
        speed = originalSpeed + amount;
        OnSpeedBuffStarted?.Invoke();

        // Wait for the duration of the speed buff. If the player picks up a speed buff whilst one is already active then
        // the duration of it is refreshed
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Speed buff has ended
        speed = originalSpeed;
        OnSpeedBuffEnded?.Invoke();

        speedBuffCoroutine = null;
    }

    /*
    
    void OnDrawGizmosSelected()
    {
        // A yellow sphere of the players pickup radius. The player needs to be selected in the editor for this to appear
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
    */
}
