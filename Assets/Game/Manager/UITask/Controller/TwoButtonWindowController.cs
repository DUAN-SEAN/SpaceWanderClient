using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoButtonWindowController : MonoBehaviour
{

    public void Open()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Closed()
    {

    }

    public void Closed(GameObject gb)
    {
        Destroy(gb);
    }

    public void OnChangeContent(string textContent,string leftButton, string rightButton)
    {
        gameObject.transform.Find("BG/Content").GetComponent<Text>().text = textContent;
        gameObject.transform.Find("BG/LeftButton/Text").GetComponent<Text>().text = leftButton;
        gameObject.transform.Find("BG/RightButton/Text").GetComponent<Text>().text = rightButton;
    }

    public void OnLeftButton()
    {
        LeftButton?.Invoke();
    }

    public void OnRightButton()
    {
        RightButton?.Invoke();
    }

    private CanvasGroup canvasGroup;

    public Action LeftButton;
    public Action RightButton;


}
