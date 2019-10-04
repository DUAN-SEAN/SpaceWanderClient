using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUpButtonController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSinup()
    {
        OnSignUpButton?.Invoke();
    }
    public event Action OnSignUpButton;
}
