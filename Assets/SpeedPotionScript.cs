using UnityEngine;

public class SpeedPotionScript : MonoBehaviour
{
    public float speedIncreaseValue = 2.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // increase player speed
            other.GetComponent<Player>().UpgradeMovementSpeed(speedIncreaseValue);
            // destroy the potion
            Destroy(gameObject);
        }
    }

}