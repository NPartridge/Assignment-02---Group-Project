using UnityEngine;

public class PotionScript : MonoBehaviour
{
    public Player playerScript;
    public int healthRestoreValue = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add health to the player
            playerScript.CurrentHealth += healthRestoreValue;
            // destroy the potion
            Destroy(gameObject);
        }
    }
}
