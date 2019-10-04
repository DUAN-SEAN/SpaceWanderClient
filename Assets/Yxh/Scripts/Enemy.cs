using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{

   
    public GameObject shot;
    public Transform shotSpawnOne;
    public Transform shotSpawnTwo;
    public GameObject missle;
    public Transform missleSpawnOne;
    public Transform missleSpawnTwo;
    public GameObject aircraft;
    public Transform aircraftSpawnOne;
    public Transform aircraftSpawnTwo;
    public float fireRate;
    private float nextFire;
    private float n = 0.0f;
    private GameObject tempObject;

    void Start()
    {
        
        tempObject = Instantiate(aircraft, aircraftSpawnOne.position, aircraftSpawnOne.rotation);
        Instantiate(aircraft, aircraftSpawnTwo.position, aircraftSpawnTwo.rotation);

        tempObject.transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y - 90f,transform.eulerAngles.z);
        //Debug.Log(GetVerticalDir(transform.position));


    }




    void Update()
    {
        //Debug.Log(tempObject.transform.rotation.eulerAngles);

        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawnOne.position, shotSpawnOne.rotation);
            Instantiate(shot, shotSpawnTwo.position, shotSpawnTwo.rotation);
            Instantiate(missle, missleSpawnOne.position, missleSpawnOne.rotation);
            Instantiate(missle, missleSpawnTwo.position, missleSpawnTwo.rotation);
        }
    }
    public static Vector3 GetVerticalDir(Vector3 v)
    {
        //（_dir.x,_dir.z）与（？，1）垂直，则_dir.x * ？ + _dir.z * 1 = 0
        if (v.x < 0.05 && v.x>-0.05)
        {
            return new Vector3(1, 0, 0);
        }
        else
        {
            return new Vector3(-v.z / v.x, 0, 1).normalized;
        }

    }


}
