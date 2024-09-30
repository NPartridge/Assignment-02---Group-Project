using UnityEngine;

public class HealthPotionScript : MonoBehaviour
{
    public int healthRestoreValue = 20;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // restore player health
            other.GetComponent<Player>().CurrentHealth += healthRestoreValue;
            // destroy the potion
            Destroy(gameObject);
        }
    }

}
