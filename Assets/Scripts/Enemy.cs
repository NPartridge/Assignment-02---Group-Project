using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform player;
    private CapsuleCollider playerCollider;
    private Rigidbody rb;
    
    [Header("AI stat sheet")]
    [SerializeField] public float speed = 3f;
    public float health = 100f;
    public float stopDistance = 1f; // Distance that the enemy will stop moving towards the player (prevent player/enemy merging)


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        playerCollider = player.GetComponent<CapsuleCollider>();
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
        
        // Stop distance based on players collider radius
        float effectiveStopDistance = playerCollider.radius + stopDistance;
        
        // Move towards player if enemy is further away than stop distsance
        if (distanceToPlayer > effectiveStopDistance)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * (speed * Time.deltaTime);
        }
    }
}

