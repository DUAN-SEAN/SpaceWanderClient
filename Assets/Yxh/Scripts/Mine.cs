using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCollider = UnityEngine.Collider;
public class Mine : MonoBehaviour
{
    public GameObject explosion;
    public Transform OverlapSphereCube;
    public float SearchRadius;
    private float mineTime = 0.0f; //放置炸弹的时间
    private int flag = 0;      //控制时间只检测一次
    void Start()
    {
        mineTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > mineTime+2 && flag == 0)
        {
            UnityCollider[] colliders = Physics.OverlapSphere(OverlapSphereCube.position, SearchRadius);
            Instantiate(explosion, transform.position, transform.rotation);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    //print(colliders[i].gameObject.name);
                    Destroy(colliders[i].gameObject);
                }              
            }
            Destroy(this.gameObject);


        }
    }
}
