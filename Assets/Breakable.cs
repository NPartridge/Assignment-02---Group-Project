using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Bullet")){
            Debug.Log("Hit");
        }
    }
}
