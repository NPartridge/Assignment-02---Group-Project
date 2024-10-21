using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] SoundEffect soundEffect;

    private float nextFire;
    public GameObject bulletPrefab;

    // This should be an empty prefab at the location where the projectiles for the weapon should be fired from
    [SerializeField] private Transform weaponFiringPoint;

    private Player playerScript;

    [Header("Weapon stat sheet")]
    [SerializeField] private float weaponAttackSpeed = 0.5f;
    [SerializeField] private int weaponDamage = 0;

    public Animator animator;
    
    private PauseManager pauseManager;

    private AudioSource audioSource;

    void Start()
    {
        //playerScript = GetComponent<Player>();
        playerScript = GetComponentInParent<Player>();
        pauseManager = FindObjectOfType<PauseManager>();

        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        // We want to avoid shooting if the player is dead
        if (playerScript == null || playerScript.IsDead)
        {
            return;
        }

        // We want to avoid shooting if the player is in a menu, this is to avoid projectiles from being initialized whilst paused
        if (pauseManager.IsAnyMenuOpen())
        {
            return;
        }

        float effectiveAttackSpeed = weaponAttackSpeed / playerScript.AttackSpeed;

        if (playerScript.AutoAimEnabled)
        {
            // Auto-aim is enabled, fire automatically
            if (Time.time > nextFire)
            {
                nextFire = Time.time + effectiveAttackSpeed;
                // Display attack animation
                animator.SetBool("autoAttackEnabled", true);

                Shoot();
            }
        }
        else
        {
            animator.SetBool("autoAttackEnabled", false);
            // Auto-aim is disabled here, only fire when the player clicks the mouse button
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
            {
                nextFire = Time.time + effectiveAttackSpeed;

                //display attack animation
                animator.SetTrigger("attack");

                Shoot();
            }
        }
    }

    private void Shoot()
    {
        int baseDamage = playerScript.TotalPlayerDamage + weaponDamage;

        // Check if the attack is a crit
        bool isCritical = Random.value <= playerScript.CritChance;

        int totalDamage = baseDamage;

        if (isCritical)
        {
            totalDamage = Mathf.RoundToInt(baseDamage * playerScript.CritDamageMultiplier);
        }

        // Play player attack sound effect
        audioSource.PlayOneShot(soundEffect.PlayerAttack);

        GameObject bullet = Instantiate(bulletPrefab, weaponFiringPoint.position, weaponFiringPoint.rotation);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();

        
        if (bulletScript != null)
        {
            bulletScript.SetDamage(totalDamage, isCritical);
        }

        if (playerScript.AutoAimEnabled)
        {
            // We get the current target from the player
            GameObject target = playerScript.CurrentTarget;

            if (target != null)
            {
                // Direction between weapon firing point and target
                Vector3 direction = (target.transform.position - weaponFiringPoint.position).normalized;
                // Send our bullet in that direction
                bullet.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            // We were are manually aiming here so we set the bullets rotation to match the weapon firing point rotation
            bullet.transform.rotation = weaponFiringPoint.rotation;
        }
    }
}