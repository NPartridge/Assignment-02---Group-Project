using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private CapsuleCollider playerCollider;
    private Rigidbody rb;
    private Animator animator;
    
    [Header("AI stat sheet")]
    [SerializeField] public float speed = 3f;
    public float health = 100f;
    public float stopDistance = 1f; // Distance that the enemy will stop moving towards the player (prevent player/enemy merging)
    public float rotationSpeed = 5f;
    
    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private float lastAttackTime = -Mathf.Infinity;



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerCollider = player.GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
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
            animator.SetBool("isMoving", true);
        }
        else
        {
            // If enemy is close enough to player, attack!
            Attack();
        }
    }
    
    void Attack()
    {
        // Check if enough time has passed since the last attack
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            // Record the time of this attack
            lastAttackTime = Time.time;

            // Trigger attack animation
            animator.SetTrigger("Attack");
        }
    }


}

