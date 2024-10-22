using System.Collections;
using UnityEngine;

public class SpeedPotionScript : MonoBehaviour
{
    [SerializeField] SoundEffect soundEffect;
    AudioSource audioSource;
    
    public float speedIncreaseValue = 2.5f;
    public float duration = 5f;

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
            // Creating a separate sound object so when we destroy the speed potion it doesn't delete the audio too
            GameObject soundGameObject = new GameObject("SpeedPotionSound");
            AudioSource newAudioSource = soundGameObject.AddComponent<AudioSource>();
            newAudioSource.clip = soundEffect.SpeedPotion;
            newAudioSource.Play();
            
            // We destroy the sound object after its played its sound
            Destroy(soundGameObject, soundEffect.SpeedPotion.length);
            
            // temporarily increase player speed
            Player player = other.GetComponent<Player>();
            player.ApplyTemporarySpeedBuff(speedIncreaseValue, duration);
            // destroy the potion
            Destroy(gameObject);
        }
    }
}