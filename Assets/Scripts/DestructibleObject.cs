using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DestructibleObject : MonoBehaviour
{
    [SerializeField] SoundEffect soundEffect;
    private AudioSource audioSource;

    // List of items dropped from the object
    public List<GameObject> dropTable;
    
    private MeshRenderer[] meshRenderers;
    private Collider[] colliders;
    private NavMeshObstacle navObstacle;
    
    public bool IsActive { get; private set; }

    private void Start()
    {
        // We need the mesh renderers and colliders for respawning the objects. To respawn the objects we will
        // just disable and re-enable the renderers/colliders 
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        colliders = GetComponentsInChildren<Collider>();
        navObstacle = GetComponent<NavMeshObstacle>();
        
        IsActive = true;

        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.CompareTag("Bullet"))
        {
            // Play collectible BarrelBreak sound effect
            audioSource.PlayOneShot(soundEffect.BarrelBreak);

            // spawn an item
            SpawnRandomDrop();
            // destroy the bullet
            Destroy(other.gameObject);
            // destroy the object
            StartCoroutine(DeactivateAndRespawn());
        }
    }

    void SpawnRandomDrop()
    {
        // chose a random item from the droptable for the object
        int itemNumber = Random.Range(0, dropTable.Count);
        // spawn the drop item at space 1 unit above the object location
        Instantiate(dropTable[itemNumber], transform.position + Vector3.up, Quaternion.identity);
    }
    
    private IEnumerator DeactivateAndRespawn()
    {
        // The object has been destroyed at this point so we are disabling everything in this section
        IsActive = false;
        
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = false;
        }
        
        foreach (var col in colliders)
        {
            col.enabled = false;
        }
        
        if (navObstacle != null)
        {
            navObstacle.enabled = false;
        }
        
        // The spawn timer for the object. We re-enable everything after this point as the object should respawn
        yield return new WaitForSeconds(10f);
        
        IsActive = true;
        
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = true;
        }
        
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
        
        if (navObstacle != null)
        {
            navObstacle.enabled = true;
        }
    }
}
