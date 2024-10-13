using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Player playerScript;
    private Transform player;
    private CapsuleCollider playerCollider;
    private CapsuleCollider enemyCollider;
    private Animator animator;
    
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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCollider = player.GetComponentInChildren<CapsuleCollider>();   GetComponent<CapsuleCollider>();
        enemyCollider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {
        if (player != null)
        {
            MoveTowardsPlayer();
        }
    }

    void MoveTowardsPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float effectiveStopDistance = playerCollider.radius + stopDistance;

        RotateTowardsPlayer();

        if (distanceToPlayer > effectiveStopDistance)
        {
            // Move forward in the direction the player is facing
            transform.position += transform.forward * (speed * Time.deltaTime);
            animator.SetBool(IsEnemyMoving, true);
        }
        else
        {
            // Check if the player is alive before attacking
            if (playerScript.CurrentHealth > 0)
            {
                // Check enemy type and choose appropriate attack method
                if (enemyType == EnemyType.Melee)
                    // If enemy is close enough to player, attack!
                    MeleeAttack();
                else if (enemyType == EnemyType.Ranged)
                    RangedAttack();
            }
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 playerDirection = player.position - transform.position;
        playerDirection.y = 0; // Keep only the horizontal direction

        if (playerDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerDirection);

            // Rotate towards player
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    void MeleeAttack()
    {
        // Check if enough time has passed since the last attack, we don't want the enemy to cast the attack animation repeatedly
        if (Time.time >= lastAttackTime + attackSpeed)
        {
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
            // Record the time of this attack
            lastAttackTime = Time.time;

            // Cast attack animation
            animator.SetTrigger(AttackAnimationTrigger);

            GameObject bullet = Instantiate(bulletPrefab, transform.position + Vector3.up, transform.rotation);
            EnemyBulletScript bulletScript = bullet.GetComponent<EnemyBulletScript>();

            if (bulletScript != null)
            {
                // Ranged Enemy does not do critical damage
                bulletScript.SetDamage(damage, false);
            }
        }
    }

    void BossAttack()
    {
        
    }

    void DamagePlayer(int amount)
    {
        playerScript.CurrentHealth -= amount;
        // Debug.Log("Enemy dealt " + amount + " damage!");
    }
    
    public void TakeDamage(int damage, bool isCritical)
    {
        if (IsDead)
            return; // If the enemy is dead we don't need to deal damage

        health -= damage;
        ShowDamageNumber(damage, isCritical);
        
        if (health <= 0f)
        {
            Die();
        }
    }
    
    private void ShowDamageNumber(int damage, bool isCritical)
    {
        // Raising the position slightly higher so the number starts somewhere around the enemies head
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
        IsDead = true;
        animator.SetTrigger(DeathAnimationTrigger);

        enemiesKilled++;
        
        enabled = false; // Disables enemy movement/attack logic etc.

        enemyCollider.enabled = false; // Disables the collider so the player can't shoot at dead enemies

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

