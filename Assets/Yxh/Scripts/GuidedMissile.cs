using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidedMissile : MonoBehaviour
{
    public GameObject target;
    private float speed = 1f;
    private float velocity = 200f;
    private Vector3 step;
    private float detonationDistance = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
        Quaternion.LookRotation(target.transform.position - transform.position), Time.deltaTime * speed);
        step = transform.forward * Time.deltaTime * velocity;
        transform.position += step;
        if (target != null && Vector3.SqrMagnitude(transform.position - target.transform.position) <= detonationDistance)
        {
            Time.timeScale = 0;
        }
        
    }
}
