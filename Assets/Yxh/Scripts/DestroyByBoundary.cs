using UnityEngine;
using System.Collections;
using UnityCollider = UnityEngine.Collider;
public class DestroyByBoundary : MonoBehaviour
{
    void OnTriggerExit(UnityCollider other)
    {
        Destroy(other.gameObject);
    }
}
