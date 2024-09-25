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

    public int WeaponDamage { get => weaponDamage; set => weaponDamage = value; }

    public Animator animator;
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
                // display autoattack animation
                animator.SetBool("autoAttackEnabled", true);
                Shoot();
            }
        }
        else
        {
            // Auto-aim is disabled here, only fire when the player clicks the mouse button
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                //display attack animation
                animator.SetTrigger("attack");
                Shoot();
            }
        }
    }

    private void Shoot()
    {
        // Get the rotation of the root of the rig 
        Quaternion rotation = GameObject.FindGameObjectWithTag("Root").transform.rotation;
        // fire the bullet in the direction that the rig is rotated
        Instantiate(bulletPrefab, weaponFiringPoint.position, rotation);

    }

}