using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public Material material;
    public float speedx = 0f;
    public float speedy = 0f;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {

        material.mainTextureOffset = new Vector2(Time.time * speedx, Time.time * speedy);
    }
}
