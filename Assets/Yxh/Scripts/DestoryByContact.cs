using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityCollider = UnityEngine.Collider;
public class DestoryByContact : MonoBehaviour
{
    public GameObject explosion;
    public GameObject playerExplosion;
    private GameController gameController;
    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null)
        {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null)
        {
           //Debug.Log("Cannot find 'GameController' script");
        }
    }
    void OnTriggerEnter(UnityCollider other)
    {
        if (other.tag == "Boundary")
        {
            return;
        }
        //Instantiate(explosion, transform.position, transform.rotation);
        if (other.tag == "Player")
        {
            //Instantiate(playerExplosion, other.transform.position, other.transform.rotation);
            gameController.GameOver();
        }
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
