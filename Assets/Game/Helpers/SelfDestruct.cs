using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;


public class SelfDestruct : MonoBehaviour
{
    public float delay = 5; // Setting this to 0 means no selfdestruct.
    
    void Start()
    {
        if (delay != 0)
        {
            Destroy(gameObject, delay);
        }
    }

    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
