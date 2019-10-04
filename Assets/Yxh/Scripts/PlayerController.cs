using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float tilt;
    public Boundary boundary;
    private new Rigidbody rigidbody;
    public GameObject shotTypeOne;         //子弹
    public GameObject shotTypeTwo;         //导弹
    public GameObject shotTypeThree;     //激光
    public GameObject shotTypeFour;        //雷
    public Transform shotSpawn;
    public Transform mineSpawn;
    public float fireRate;
    private GameObject tempLighting;
    private float nextFire;
    private float n=0.0f;
    private new AudioSource audio;
    private int shotType=1;
    private bool isLighting = false;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }




    void Update()
    {

        if(Input.GetKey(KeyCode.Alpha1))
        {
            shotType = 1;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            shotType = 2;
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            shotType = 3;
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            shotType = 4;
        }
        if ((Input.GetButton("Fire1") ||Input.GetKey(KeyCode.Space))&& Time.time > nextFire && shotType == 1)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shotTypeOne, shotSpawn.position, shotSpawn.rotation);
            audio.Play();
        }

        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && Time.time > nextFire && shotType == 2)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shotTypeTwo, shotSpawn.position, shotSpawn.rotation);
            audio.Play();
        }
        if(Input.GetMouseButton(0) && shotType == 3)
        {
            if(!isLighting)
            {
                tempLighting =Instantiate(shotTypeThree, shotSpawn.position, shotSpawn.rotation);
                tempLighting.transform.parent = this.gameObject.transform;
                audio.Play();
                isLighting = true;
            }
        }
        if(Input.GetMouseButtonUp(0) && shotType == 3)
        {
            if(isLighting)
            {
                Destroy(tempLighting);
                isLighting = false;
            }
        }


        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && Time.time > nextFire && shotType == 2)
        {
            nextFire = Time.time + fireRate;
        }

        if ((Input.GetButton("Fire1") || Input.GetKey(KeyCode.Space)) && Time.time > nextFire && shotType == 4)
        {
            nextFire = Time.time + fireRate;
            Instantiate(shotTypeFour, mineSpawn.position, shotSpawn.rotation);
            audio.Play();
        }
    }
    void FixedUpdate()
    {
        MyRotation();
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rigidbody.velocity = movement * speed;

        rigidbody.position = new Vector3
        (
            Mathf.Clamp(rigidbody.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rigidbody.position.z, boundary.zMin, boundary.zMax)
        );
        rigidbody.rotation = Quaternion.Euler(0.0f, n, rigidbody.velocity.x * -tilt);
    }

    private void MyRotation()
    {
        if(Input.GetKey(KeyCode.Q))
        {
            n=n-2;
            Debug.Log("Q"); 
        }
        if(Input.GetKey(KeyCode.E))
        {
            n=n+5;
            Debug.Log("E");
        }


    }
} 