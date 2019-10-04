using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignUpController : MonoBehaviour
{

    public void OnCancel()
    {
        OnCancelButton?.Invoke();
    }

    public void OnSinup()
    {
        OnSignUpButton?.Invoke();
    }

    public event Action OnCancelButton;
    public event Action OnSignUpButton;
}
