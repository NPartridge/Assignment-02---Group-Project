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
            audioSource.PlayOneShot(soundEffect.SpeedPotion);
            // temporarily increase player speed
            Player player = other.GetComponent<Player>();
            player.ApplyTemporarySpeedBuff(speedIncreaseValue, duration);
            // destroy the potion
            Destroy(gameObject);
        }
    }
}