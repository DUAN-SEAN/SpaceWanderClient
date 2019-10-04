using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShipHouseController : MonoBehaviour
{

    public void Open()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        
    }

    public void Closed()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnChangeShip()
    {
        OnChangeShipButton?.Invoke();
    }

    public void OnChangeWeapon1()
    {
        OnChangeWeapon1Button?.Invoke();
    }

    public void OnChangeWeapon2()
    {
        OnChangeWeapon2Button?.Invoke();
    }

    public void OnSaveChanges()
    {
        OnSaveChangesButton?.Invoke();
    }

    public void OnCancel()
    {
        OnCancelButton?.Invoke();
    }

    public void UpdatePlayerShip(string ship,string weapon1,string weapon2)
    {
        gameObject.transform.Find("TextContentGroup/ShipContentText").GetComponent<Text>().text = ship;
        gameObject.transform.Find("TextContentGroup/Weapon1ContentText").GetComponent<Text>().text = weapon1;
        gameObject.transform.Find("TextContentGroup/Weapon2ContentText").GetComponent<Text>().text = weapon2;
    }


    private CanvasGroup canvasGroup;
    

    public static string _whichBeChosen = null;

    public event Action OnChangeShipButton;
    public event Action OnChangeWeapon1Button;
    public event Action OnChangeWeapon2Button;

    public event Action OnSaveChangesButton;
    public event Action OnCancelButton;
}
