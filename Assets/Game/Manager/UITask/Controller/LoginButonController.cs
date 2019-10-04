using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginButonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLogin()
    {
        OnLoginButton?.Invoke();
    }

    public event Action OnLoginButton;
}
