using UnityEngine;

public class HealthPotionScript : MonoBehaviour
{
    public int healthRestoreValue = 20;

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
            // restore player health
            other.GetComponent<Player>().CurrentHealth += healthRestoreValue;
            // destroy the potion
            Destroy(gameObject);
        }
    }

}

