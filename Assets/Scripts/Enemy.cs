using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Player playerScript;
    public Transform player;
    private CapsuleCollider playerCollider;
    private Animator animator;
    
    [Header("AI stat sheet")]
    [SerializeField] public float speed = 3f;
    [SerializeField] private int damageAmount = 5;
    public int health = 100;
    public float stopDistance = 1f; // Distance that the enemy will stop moving towards the player (prevent player/enemy merging)
    public float rotationSpeed = 5f;
    public float attackCooldown = 2f;
    
    
    private float lastAttackTime = -Mathf.Infinity;
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int Attack1 = Animator.StringToHash("Attack");

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCollider = player.GetComponent<CapsuleCollider>();
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
            // Rotate towards the player
            Vector3 playerDirection = player.position - transform.position;
            playerDirection.y = 0; // Keep only the horizontal direction

            if (playerDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(playerDirection);

                // Rotate towrads player
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            // Move forward in the direction the player is facing
            transform.position += transform.forward * (speed * Time.deltaTime);
            animator.SetBool(IsMoving, true);
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
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Record the time of this attack
            lastAttackTime = Time.time;

            // Cast attack animation
            animator.SetTrigger(Attack1);

            // damage the player
            DamagePlayer(damageAmount);
        }
    }

    void DamagePlayer(int amount)
    {
        playerScript.Health -= amount;
    }
}

