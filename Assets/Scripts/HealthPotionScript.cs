using UnityEngine;

public class HealthPotionScript : MonoBehaviour
{
    [SerializeField] SoundEffect soundEffect;
    AudioSource audioSource;
    public int healthRestoreValue = 20;

    BounceClass bounceClass;

    private void Start()
    {
        bounceClass = GetComponent<BounceClass>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        bounceClass.Bounce();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            audioSource.PlayOneShot(soundEffect.HealthPotion);
            // restore player health
            other.GetComponent<Player>().CurrentHealth += healthRestoreValue;
            // destroy the potion after short delay for sound effect to play
            Destroy(gameObject, 0.3f);
        }
    }

}

