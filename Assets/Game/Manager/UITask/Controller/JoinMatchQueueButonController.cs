using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinMatchQueueButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnJoinMatchQueueButton()
    {
        OnJoinMatchQueue?.Invoke();
    }

    public event Action OnJoinMatchQueue;
}
