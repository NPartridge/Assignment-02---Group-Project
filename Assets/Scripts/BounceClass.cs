using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceClass : MonoBehaviour
{
    public float minBounceHeightVariation = 0.05f;
    public float maxBounceHeightVariation = 0.5f;

    public float minBounceSpeedVariation = 0.2f;
    public float maxBounceSpeedVariation = 0.3f;

    private float bounceHeight = 0.2f;
    private float bounceSpeed = 1f;
    private float bounceOffset = 0.5f; // This is to help the bounce feel less uniform, more info about this in Bounce()
    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;

        bounceHeight += Random.Range(minBounceHeightVariation, maxBounceHeightVariation);
        bounceSpeed += Random.Range(minBounceHeightVariation, maxBounceHeightVariation);
    }

    public void Bounce()
    {
        // Calculate the new y pos using a sine wave with added randomness
        // The code comes from here (small volume warning, the song he uses is kinda loud)
        // How To Move Obstacles Or Even Enemy In Sine Wave Way Up And Down With Simple C# Script In Unity Game https://www.youtube.com/watch?v=UVqH2XS5E2M
        float newY = initialPosition.y + Mathf.Sin(Time.time * bounceSpeed + bounceOffset) * bounceHeight;
        Vector3 newPosition = new Vector3(initialPosition.x, newY, initialPosition.z);
        transform.position = newPosition;
    }
}
