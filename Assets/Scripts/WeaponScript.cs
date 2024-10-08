using UnityEngine;

public class WeaponScript : MonoBehaviour
{
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

    void Start()
    {
        //playerScript = GetComponent<Player>();
        playerScript = GetComponentInParent<Player>();
        pauseManager = FindObjectOfType<PauseManager>();
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

        GameObject bullet = Instantiate(bulletPrefab, weaponFiringPoint.position, weaponFiringPoint.rotation);
        BulletScript bulletScript = bullet.GetComponent<BulletScript>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(totalDamage, isCritical);
        }
    }
}