using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    private float nextFire;
    public GameObject bulletPrefab;
    
    // This should be an empty prefab at the location where the projectiles for the weapon should be fired from
    [SerializeField] private Transform weaponFiringPoint;
    
    private Player playerScript;
    
    [Header("Weapon stat sheet")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int weaponDamage;

    public int WeaponDamage => weaponDamage;

    void Start()
    {
        //playerScript = GetComponent<Player>();
        playerScript = GetComponentInParent<Player>();
    }

    void Update()
    {
        if (playerScript.AutoAimEnabled)
        {
            // Auto-aim is enabled, fire automatically
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }
        }
        else
        {
            // Auto-aim is disabled here, only fire when the player clicks the mouse button
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, weaponFiringPoint.position, weaponFiringPoint.rotation);
    }
}