using System.Collections;
using UnityEngine;

public class SpeedPotionScript : MonoBehaviour
{
    public float speedIncreaseValue = 2.5f;
    public float duration = 5f;

    BounceClass bounceClass;
    private void Start()
    {
        bounceClass = GetComponent<BounceClass>();
    }

    private void Update()
    {
        bounceClass.Bounce();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // temporarily increase player speed
            Player player = other.GetComponent<Player>();
            player.StartCoroutine(player.ApplyTemporarySpeedBuff(speedIncreaseValue, duration));
            // destroy the potion
            Destroy(gameObject);
        }
    }
}