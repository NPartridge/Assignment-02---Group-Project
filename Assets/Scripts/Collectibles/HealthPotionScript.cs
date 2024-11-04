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
            // Creating a separate sound object so when we destroy the speed potion it doesn't delete the audio
            GameObject soundGameObject = new GameObject("HealthPotionSound");
            AudioSource newAudioSource = soundGameObject.AddComponent<AudioSource>();
            newAudioSource.clip = soundEffect.HealthPotion;
            newAudioSource.Play();
        
            // We destroy the sound object after its played its sound
            Destroy(soundGameObject, soundEffect.HealthPotion.length);
        
            // Restore the players HP
            other.GetComponent<Player>().CurrentHealth += healthRestoreValue;
        
            // Destroy the potion immediately
            Destroy(gameObject);
        }
    }
}

