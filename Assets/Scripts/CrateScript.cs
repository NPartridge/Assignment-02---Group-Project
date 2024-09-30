using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    // List of items dropped from a crate
    public List<GameObject> dropTable;

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Bullet")){
            // spawn an item
            SpawnRandomDrop();
            // destroy the bullet
            Destroy(other.gameObject);
            // destroy the box
            Destroy(gameObject);
        }
    }

    void SpawnRandomDrop()
    {
        // chose a random item from the droptable for a crate
        int itemNumber = Random.Range(0, dropTable.Count);
        // spawn the drop item at space 1 unit above the crate location
        Instantiate(dropTable[itemNumber], transform.position + Vector3.up, Quaternion.identity);
    }
}
