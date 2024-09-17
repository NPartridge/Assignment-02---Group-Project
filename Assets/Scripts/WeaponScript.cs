using UnityEngine;

// Code for timer from: https://docs.unity3d.com/ScriptReference/Time-time.html

public class WeaponScript : MonoBehaviour
{
    [SerializeField] float fireRate = 0.5f;
    private float nextFire = 0.0f;

    public GameObject bulletPrefab;
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 weaponOffset = new Vector3(0, 0.9f, 0);
        Instantiate(bulletPrefab, transform.position + weaponOffset, transform.rotation);
    }
}
