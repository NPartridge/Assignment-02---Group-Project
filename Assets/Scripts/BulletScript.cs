using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    void Start()
    {

    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
