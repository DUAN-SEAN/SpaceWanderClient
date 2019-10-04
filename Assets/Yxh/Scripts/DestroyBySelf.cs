using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBySelf : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("destroyself", 2);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void destroyself()
    {
        Destroy(this.gameObject); 
    }

}
