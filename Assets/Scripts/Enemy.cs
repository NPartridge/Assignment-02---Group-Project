using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] SoundEffect soundEffect;
    private AudioSource audioSource;

    private Player playerScript;
    private Transform player;
    private CapsuleCollider playerCollider;
    private CapsuleCollider enemyCollider;
    private Animator animator;
    private NavMeshAgent navMeshAgent;

    public float stopDistance = 1f; // Distance that the enemy will stop moving towards the player (prevent player/enemy merging)
    public float rotationSpeed = 5f;
    public GameObject bulletPrefab;

    public enum EnemyType { Melee, Ranged, Boss, Elite }
    public EnemyType enemyType = EnemyType.Melee;

    [Header("Enemy stat sheet")]
    [SerializeField] public float speed = 3f;
    [SerializeField] private int damage = 5;
    public int health = 100;
    public float attackSpeed = 2f;
    public GameObject gemPrefab; // The gem the enemy will drop on death
    public static int enemiesKilled = 0;

    private float lastAttackTime = -Mathf.Infinity;
    private static readonly int IsEnemyMoving = Animator.StringToHash("isMoving");
    private static readonly int AttackAnimationTrigger = Animator.StringToHash("Attack");
    private static readonly int DeathAnimationTrigger = Animator.StringToHash("Die");

    [SerializeField] private GameObject damageNumberPrefab;

    public bool IsDead { get; private set; }

    public Transform firingPoint;
    public GameObject gunPrefab;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCollider = player.GetComponentInChildren<CapsuleCollider>(); GetComponent<CapsuleCollider>();
        enemyCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        playerScript = player.GetComponent<Player>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.stoppingDistance = stopDistance;
        navMeshAgent.speed = speed;
        navMeshAgent.radius = enemyCollider.radius;
        navMeshAgent.avoidancePriority = Random.Range(0, 100); // How much the enemy will avoid other enemies

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (player != null && !IsDead)
        {
            // The agents destination is the player
            navMeshAgent.SetDestination(player.position);
            animator.SetBool(IsEnemyMoving, navMeshAgent.velocity.sqrMagnitude > 0.1f);

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Here we are checking if the agent has reached their stopping distance
            if (distanceToPlayer <= navMeshAgent.stoppingDistance + 0.1f)
            {
                RotateTowardsPlayer();

                if (playerScript.CurrentHealth > 0)
                {
                    if (enemyType == EnemyType.Melee)
                        MeleeAttack();
                    else if (enemyType == EnemyType.Ranged)
                        RangedAttack();
                }
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0f; // Keep only the horizontal direction

        if (direction != Vector3.zero)
        {
            // Rotate towards player
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void MeleeAttack()
    {
        // Check if enough time has passed since the last attack, we don't want the enemy to cast the attack animation repeatedly
        if (Time.time >= lastAttackTime + attackSpeed)
        {
            // Play enemy MeleeAttack sound effect
            audioSource.PlayOneShot(soundEffect.MeleeAttack);
            // Record the time of this attack
            lastAttackTime = Time.time;

            // Cast attack animation
            animator.SetTrigger(AttackAnimationTrigger);

            // damage the player
            DamagePlayer(damage);
        }
    }

    void RangedAttack()
    {

        if (Time.time >= lastAttackTime + attackSpeed)
        {
            // Play enemy RangedAttack sound effect
            audioSource.PlayOneShot(soundEffect.RangedAttack);

            // Record the time of this attack
            lastAttackTime = Time.time;

            // Cast attack animation
            animator.SetTrigger(AttackAnimationTrigger);

            if (firingPoint == null)
            {
                Debug.Log("No Firing Point");
            }

            // Get the rotation towards the player
            Vector3 playerDirection = (player.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(playerDirection);

            // rotate the firing point using that rotation
            firingPoint.rotation = rotation;

            // change the local rotation of the gun model on the x axis only
            Quaternion gunRotation = Quaternion.Euler(firingPoint.rotation.x, gunPrefab.transform.rotation.y, gunPrefab.transform.rotation.z);
            gunPrefab.transform.localRotation = gunRotation;

            GameObject bullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);

            // The boss ranged enemy is using the bulletSpray prefab which is composed of three bullets each with a bullet script attached
            // We need to loop through each script attached to a bullet child of the prefab and set the damage
            if (transform.name == "BossZombieSoldier")
            {
                EnemyBulletScript[] bulletScripts = bullet.GetComponentsInChildren<EnemyBulletScript>();
                foreach (EnemyBulletScript b in bulletScripts)
                {
                    b.SetDamage(damage, false);
                }
            }
            else
            {
                EnemyBulletScript bulletScript = bullet.GetComponent<EnemyBulletScript>();
                if (bulletScript != null)
                {
                    // Ranged Enemy does not do critical damage
                    bulletScript.SetDamage(damage, false);
                }

            }


        }
    }

    void BossAttack()
    {

    }

    void DamagePlayer(int amount)
    {
        // Play player playerHurt sound effect
        audioSource.PlayOneShot(soundEffect.PlayerHurt);

        playerScript.CurrentHealth -= amount;
        // Debug.Log("Enemy dealt " + amount + " damage!");
    }

    public void TakeDamage(int damage, bool isCritical)
    {
        if (IsDead)
            return; // If the enemy is dead we don't need to deal damage

        // Play enemy enemyHurt sound effect
        audioSource.PlayOneShot(soundEffect.EnemyHurt);
        health -= damage;
        ShowDamageNumber(damage, isCritical);

        if (health <= 0f)
        {
            Die();
        }
    }

    private void ShowDamageNumber(int damage, bool isCritical)
    {
        // Raising the position slightly higher so the number starts somewhere around the enemies head. The damage numbers
        // don't work perfectly with larger enemies like bosses. This could be refactored so each enemy has its own
        // damage number position but this is probably unnecessary for now
        GameObject dmgNumber = Instantiate(damageNumberPrefab, transform.position + Vector3.up * 2f, Quaternion.identity);

        dmgNumber.transform.SetParent(transform);

        // Set the amount of damgae
        DamageNumber dmgNumberScript = dmgNumber.GetComponent<DamageNumber>();
        if (dmgNumberScript != null)
        {
            dmgNumberScript.DisplayDamage(damage, isCritical);
        }
        else
        {
            Debug.LogError("DMG number script not on prefab");
        }
    }



    void Die()
    {
        // Play enemy enemyDie sound effect
        audioSource.PlayOneShot(soundEffect.EnemyDie);

        IsDead = true;
        animator.SetTrigger(DeathAnimationTrigger);

        enemiesKilled++;

        enabled = false; // Disables enemy movement/attack logic etc.

        enemyCollider.enabled = false; // Disables the collider so the player can't shoot at dead enemies
        navMeshAgent.enabled = false; // Disables the nav mesh so the enemies don't move after they're dead

        // Wait until animation is done before destroying the enemy
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(3f);

        // Drop a gem when the enemy dies. Move this line before WaitForSeconds() if we want the gems to spawn as soon as the enemy dies
        Instantiate(gemPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);

        Destroy(gameObject);
    }
}

