using UnityEngine;

public class Gem : MonoBehaviour
{
    public enum GemType { Experience, Vacuum }
    public GemType gemType = GemType.Experience;

    public float moveSpeed = 10f;
    public int experiencePoints = 10;
    public float gemSpeedMultiplier = 2f; // Multiplier for gem movement speed when vacuum is activated

    private bool isMovingToPlayer;
    private Transform playerTransform;
    private Player playerScript;
    private float currentMoveSpeed;

    private float bounceHeight = 0.2f;
    private float bounceSpeed = 1f;
    private float bounceOffset = 0.5f; // This is to help the bounce feel less uniform, more info about this in Bounce()
    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;

        currentMoveSpeed = moveSpeed;

        // Add some randomness to the gems bounce so they dont all bounce in sync
        if (gemType == GemType.Experience)
        {
            bounceHeight += Random.Range(-0.05f, 0.5f);
            bounceSpeed += Random.Range(-0.2f, 0.3f);
        }
        else if (gemType == GemType.Vacuum)
        {
            bounceHeight += Random.Range(-0.05f, 0.1f);
            bounceSpeed += Random.Range(-0.2f, 0.3f);
        }
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
            
            // Check if the gem is within the pickup radius
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= playerScript.pickupRadius)
            {
                if (gemType == GemType.Experience)
                {
                    // If gem is within pickup radius, start moving the gem towards player
                    StartMovingGemToPlayer();
                }
                else if (gemType == GemType.Vacuum)
                {
                    isMovingToPlayer = true;
                }
            }
        }
        else
        {
            MoveGemTowardsPlayer();
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

    void MoveGemTowardsPlayer()
    {
        // Move gem towards player
        float step = currentMoveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, step);

        // Check if gem has reached the player (or near enough)
        if (Vector3.Distance(transform.position, playerTransform.position) < 0.2f)
        {
            if (gemType == GemType.Experience)
            {
                // Once the gem is close enough, give the player their exp and destroy the gem. We want the experience to be counted once the gem has reached the player
                playerScript.AddExperience(experiencePoints);

                // Destroy the gem
                Destroy(gameObject);
            }
            else if (gemType == GemType.Vacuum)
            {
                // Once the vacuum is close enough, suck in all the gems
                ActivateVacuum();

                // Destroy the vacuum
                Destroy(gameObject);
            }
        }
    }

    public void StartMovingGemToPlayer(float speedMultiplier = 1f)
    {
        if (!isMovingToPlayer)
        {
            isMovingToPlayer = true;
            currentMoveSpeed = moveSpeed * speedMultiplier;
            //Debug.Log("Gem moving towards player with speed multiplier " + speedMultiplier);
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void ActivateVacuum()
    {
        // Find all existing gems in the scene
        Gem[] allGems = FindObjectsOfType<Gem>();

        // For each gem, tell it to start moving towards the player
        foreach (Gem gem in allGems)
        {
            // Only suck in experience gems
            if (!gem.isMovingToPlayer && gem.gemType == GemType.Experience)
            {
                gem.StartMovingGemToPlayer(gemSpeedMultiplier);
            }
        }

        Debug.Log("Vacuum activated!");
    }
}
