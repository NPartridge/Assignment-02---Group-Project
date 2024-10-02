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

    void Start()
    {

        currentMoveSpeed = moveSpeed;

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
            GetComponent<BounceClass>().Bounce();
            
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
