using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class OrbitalBeamLaser : MonoBehaviour
{
    
    public GameObject LaserEffects;
    public ParticleSystem LaserSparks;
    public ParticleSystem LaserSmoke ;
    public AudioSource LaserChargeAudio ;
    public AudioSource LaserAudio;
    public AudioSource LaserStopAudio;
    public GameObject LaserChargeBeam;
    public GameObject SmokeAndSparks;
    public GameObject ScorchMark;

    private GameObject ScorchMarkClone;
    private bool LaserSparksEmitter;
    private bool LaserSmokeEmitter;
    private int LaserChargeFlag  = 0;
    private float time=0,Distance=0;
    private RaycastHit hit;
    private GameObject ship;
    private GameObject shipShotPoint;
    // Start is called before the first frame update
    void Start()
    {
        LaserEffects.SetActive(false);
        //LaserSparksEmitter = LaserSparks.emission;
        //LaserSparksEmitter.enabled = false;
        //LaserSmokeEmitter = LaserSmoke.emission;
        //LaserSmokeEmitter.enabled = false;
        LaserChargeBeam.SetActive(false);
        SmokeAndSparks.SetActive(false);
        //SmokeAndSparks.SetActive(true);
        ScorchMarkClone = Instantiate(ScorchMark);
        LaserChargeAudio.Stop();
        LaserAudio.Stop();
        LaserStopAudio.Stop();
        ship = GameObject.Find("Ship");
        shipShotPoint = ship.transform.Find("ShotPoint").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = shipShotPoint.transform.position;
        transform.rotation = shipShotPoint.transform.rotation;
        //Debug.DrawRay(transform.position, transform.up * 10000f, Color.red);
        // Fire laser when left mouse button is pressed
        if (Input.GetButtonDown("Fire1"))
        {
            //CalculatingDistance();
            LaserChargeFlag = 0;
            LaserChargeAudio.Play();
            LaserChargeBeam.SetActive(true);
            time += Time.time;
            StartCoroutine(MyMethod());
        }
      
        // Stop laser if left mouse button is released

        if (Input.GetButtonUp("Fire1"))
        {

            LaserChargeFlag = 1;
            LaserEffects.SetActive(false);
            //LaserSparksEmitter.enabled = false;
            //LaserSmokeEmitter.enabled = false;
            LaserAudio.Stop();
            LaserStopAudio.Play();
            SmokeAndSparks.SetActive(false);
            ScorchMark.SetActive(false);
            LaserChargeBeam.SetActive(false);
        }
        
    }
    void CalculatingDistance()
    {
        bool grounded = Physics.Raycast(transform.position, Vector3.up, out hit);
        // 可控制投射距离bool grounded = Physics.Raycast(transform.position, -Vector3.up, out hit,100.0);
        if (grounded)
        {
            Debug.Log("发生了碰撞");
            Debug.Log("距离是：" + hit.distance);
            Debug.Log("被碰撞的物体是：" + hit.collider.gameObject.name);
            Distance=hit.distance;

        }
        else
        {
            Debug.Log("碰撞结束");
        }

    }
    void LaserChargeWait()
    {
        

        if (LaserChargeFlag == 0)
        {
            LaserEffects.SetActive(true);
            //LaserSparksEmitter.enabled = true;
            //LaserSmokeEmitter.enabled = true;
            LaserAudio.Play();
            //yield WaitForSeconds (0.2);
            SmokeAndSparks.SetActive(true);
            ScorchMark.SetActive(true);
            LaserChargeFlag = 0;
        }
    }
    IEnumerator MyMethod()
    {
        yield return new WaitForSeconds(1.4f);
        LaserChargeWait();
    }
}
