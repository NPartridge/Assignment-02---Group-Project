using UnityEngine;

public class Gem : MonoBehaviour
{
    public float moveSpeed = 10f;
    public int experiencePoints = 10;

    private bool isMovingToPlayer;
    private Transform playerTransform;
    private Player playerScript;
    
    private float bounceHeight = 0.2f;
    private float bounceSpeed = 1f;
    private float bounceOffset = 0.5f; // This is to help the bounce feel less uniform, more info about this in Bounce()    
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
        
        // Add some randomness to the gems bounce so they dont all bounce in sync
        bounceHeight += Random.Range(-0.05f, 0.1f);
        bounceSpeed += Random.Range(-0.2f, 0.3f);
        bounceOffset = Random.Range(0, 5);
        
        // Find the player in the scene and get the player position/script
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerScript = playerObject.GetComponent<Player>();
        }
    }

    void Update()
    {
        // We need a player and a player script
        if (playerTransform == null || playerScript == null)
            return;

        if (!isMovingToPlayer)
        {
            // If the gem isn't moving towards the player, keep bouncing
            Bounce();
            
            // Checking the distance between gem and palyer
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= playerScript.pickupRadius)
            {
                // If gem is within pickup radius, start moving towards player
                isMovingToPlayer = true;
            }
        }

        if (isMovingToPlayer)
        {
            // Move gem towards player
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);

            // Check if gem has reached the player (or near enough)
            if (Vector3.Distance(transform.position, playerTransform.position) < 0.2f)
            {
                // Once the gem is close enough, give the player their exp and destroy the gem. We want the experience to be counted once the gem has reached the player
                playerScript.AddExperience(experiencePoints);
                Destroy(gameObject);
            }
        }
    }
    
    void Bounce()
    {
        // Calculate the new y pos using a sine wave with added randomness
        // The code comes from here (small volume warning, the song he uses is kinda loud)
        // How To Move Obstacles Or Even Enemy In Sine Wave Way Up And Down With Simple C# Script In Unity Game https://www.youtube.com/watch?v=UVqH2XS5E2M
        float newY = initialPosition.y + Mathf.Sin(Time.time * bounceSpeed + bounceOffset) * bounceHeight;
        Vector3 newPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
        transform.position = newPosition;
    }
}
