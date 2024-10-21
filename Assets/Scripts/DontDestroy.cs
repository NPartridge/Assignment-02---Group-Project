using UnityEngine;

// Reference: https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html
public class DontDestroy : MonoBehaviour
{
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("MusicPlayer");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}
