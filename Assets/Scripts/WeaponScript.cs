using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    public GameObject bulletPrefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector3 weaponOffset = new Vector3(0, 0.9f, 0);
        Instantiate(bulletPrefab, transform.position + weaponOffset, transform.rotation);
    }
}
