using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Player playerScript;
    public Transform player;
    private CapsuleCollider playerCollider;
    private CapsuleCollider enemyCollider;
    private Animator animator;
    
    public float stopDistance = 1f; // Distance that the enemy will stop moving towards the player (prevent player/enemy merging)
    public float rotationSpeed = 5f;
    
    [Header("AI stat sheet")]
    [SerializeField] public float speed = 3f;
    [SerializeField] private int damage = 5;
    public int health = 100;
    public float attackSpeed = 2f;
    
    public GameObject gemPrefab; // The gem the enemy will drop on death
    
    private float lastAttackTime = -Mathf.Infinity;
    private static readonly int IsEnemyMoving = Animator.StringToHash("isMoving");
    private static readonly int AttackAnimationTrigger = Animator.StringToHash("Attack");
    private static readonly int DeathAnimationTrigger = Animator.StringToHash("Die");
    public bool IsDead { get; private set; }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCollider = player.GetComponent<CapsuleCollider>();
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

        if (distanceToPlayer > effectiveStopDistance)
        {
            Vector3 playerDirection = player.position - transform.position;
            playerDirection.y = 0; // Keep only the horizontal direction

            if (playerDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(playerDirection);

                // Rotate towards player
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            // Move forward in the direction the player is facing
            transform.position += transform.forward * (speed * Time.deltaTime);
            animator.SetBool(IsEnemyMoving, true);
        }
        else
        {
            // Check if the player is alive before attacking
            if (playerScript.Health > 0)
            {
                // If enemy is close enough to player, attack!
                Attack();
            }
        }
    }
    
    void Attack()
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

    void DamagePlayer(int amount)
    {
        playerScript.Health -= amount;
        // Debug.Log("Enemy dealt " + amount + " damage!");
    }
    
    public void TakeDamage(int damage)
    {
        if (IsDead)
            return; // If the enemy is dead we don't need to deal damage

        health -= damage;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        IsDead = true;
        animator.SetTrigger(DeathAnimationTrigger);
        
        enabled = false; // Disables enemy movement/attack logic etc.

        enemyCollider.enabled = false; // Disables the collider so the player can't shoot at dead enemies

        // Wait until animation is done before destroying the enemy
        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(3f);
        
        // Drop a gem when the enemy dies. Move this line before WaitForSeconds() if we want the gems to spawn as soon as the enemy dies
        Instantiate(gemPrefab, transform.position, Quaternion.identity);
        
        Destroy(gameObject);
    }
}

