using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginController : MonoBehaviour
{

    public void Open()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Closed()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnOpenRegister()
    {
        OnOpenRegisterButton?.Invoke();
    }

    public void OnLogin()
    {
        OnLoginButton?.Invoke();
    }

    public void SwitchBlocksRaycasts(bool b)
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = b;
    }

    private CanvasGroup canvasGroup;

    public event Action OnLoginButton;
    public event Action OnOpenRegisterButton ;
}
