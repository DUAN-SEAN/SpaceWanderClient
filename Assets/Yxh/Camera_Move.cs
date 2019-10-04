using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Move : MonoBehaviour
{
    private Transform PlayerPosition;
    private Vector3 offset;
    private GameObject tempobject;
    private bool isbind = false;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        tempobject = GameObject.Find("Player");
        if (tempobject != null&&!isbind)
        {
            offset = transform.position - tempobject.transform.position;
            isbind = true;
        }
        else
        {
            
        }
        transform.position = tempobject.transform.position + offset;

    }
}
