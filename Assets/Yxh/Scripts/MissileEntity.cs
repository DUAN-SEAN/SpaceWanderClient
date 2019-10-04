using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCollider = UnityEngine.Collider;
public class MissileEntity : MonoBehaviour
{
    public GameObject explosion;
    private Transform OverlapSphereCube;
    public float SearchRadius;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(UnityCollider other)
    {
        if (other.tag == "Boundary")
        {
            return;
        }
        Debug.Log("爆炸");
        UnityCollider[] colliders = Physics.OverlapSphere(OverlapSphereCube.position, SearchRadius);

        if (colliders.Length <= 0)
            return;
        Instantiate(explosion, transform.position, transform.rotation);
        for (int i = 0; i < colliders.Length; i++)
        {
            //print(colliders[i].gameObject.name);
            Destroy(colliders[i].gameObject);
        }
        Destroy(this.gameObject);
    }

}
