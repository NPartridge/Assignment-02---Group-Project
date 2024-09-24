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

    void Start()
    {
        //playerScript = GetComponent<Player>();
        playerScript = GetComponentInParent<Player>();
    }

    void Update()
    {
        float effectiveAttackSpeed = weaponAttackSpeed / playerScript.AttackSpeed;
        
        if (playerScript.AutoAimEnabled)
        {
            // Auto-aim is enabled, fire automatically
            if (Time.time > nextFire)
            {
                nextFire = Time.time + effectiveAttackSpeed;
                Shoot();
            }
        }
        else
        {
            // Auto-aim is disabled here, only fire when the player clicks the mouse button
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
            {
                nextFire = Time.time + effectiveAttackSpeed;
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
        Debug.Log(isCritical ? $"Critical Hit! Damage: {totalDamage}" : $"Normal Hit. Damage: {totalDamage}");
    }
}