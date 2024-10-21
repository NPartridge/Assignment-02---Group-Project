using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Player Sound FX")]
    [SerializeField] AudioClip playerAttack;
    [SerializeField] AudioClip playerHurt;
    [SerializeField] AudioClip playerDie;

    [Header("Enemy Sound FX")]
    [SerializeField] AudioClip meleeAttack;
    [SerializeField] AudioClip rangedAttack;
    [SerializeField] AudioClip enemyHurt;
    [SerializeField] AudioClip enemyDie;

    [Header("Collectible Sound FX")]
    [SerializeField] AudioClip barrelBreak;
    [SerializeField] AudioClip healthPotion;
    [SerializeField] AudioClip speedPotion;
    [SerializeField] AudioClip experienceGemCollect;
    [SerializeField] AudioClip vacuumGemCollect;

    [Header("Game Event Sound FX")]
    [SerializeField] AudioClip levelUp;
    [SerializeField] AudioClip buttonClick;


    // Player Sound FX
    public AudioClip PlayerAttack { get => playerAttack; }
    public AudioClip PlayerHurt { get => playerHurt; }
    public AudioClip PlayerDie { get => playerDie; }


    // Enemy Sound FX
    public AudioClip MeleeAttack { get => meleeAttack; }
    public AudioClip RangedAttack { get => rangedAttack; }
    public AudioClip EnemyHurt { get => enemyHurt; }
    public AudioClip EnemyDie { get => enemyDie; }


    // Collectible Sound FX
    public AudioClip BarrelBreak { get => barrelBreak; }
    public AudioClip HealthPotion { get => healthPotion; }
    public AudioClip SpeedPotion { get => speedPotion; }
    public AudioClip ExperienceGemCollect { get => experienceGemCollect; }
    public AudioClip VacuumGemCollect { get => vacuumGemCollect; }
    
    // Event Sound FX
    public AudioClip LevelUp { get => levelUp;}
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Click()
    {
        audioSource.PlayOneShot(buttonClick);
    }
}
