using UnityEngine;
using System.Collections;

public class Done_Mover : MonoBehaviour
{
	public float speed;
    private float time;
	void Start ()
	{
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
        time = Time.time;

	}
    void Update()
    {
        
    }
}
