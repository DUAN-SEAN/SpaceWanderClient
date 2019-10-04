using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private int fuseLightIntensity = 10;
    private Light light;
    // Start is called before the first frame update
    void Start()
    {
        light = this.GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        fuseLightIntensity = (Random.Range(5, 14));
        light.intensity = fuseLightIntensity;
    }
}
