using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LevelDetailController : MonoBehaviour
{
    public void Open(List<string> str)
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        
        UpdateLevelInfo(str);

        LevelName = gameObject.transform.Find("Panel/Level_name").GetComponent<Text>().text;
    }

    public void Closed()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void UpdateLevelInfo(List<string> str)
    {
        gameObject.transform.Find("Panel/Level_name").GetComponent<Text>().text = str[0];
        gameObject.transform.Find("Panel/Level_info").GetComponent<Text>().text = str[1];


    }

    public void OnJoinMatchQueueButton()
    {
        OnJoinMatchQueueEventButton?.Invoke();
    }

    public string LevelName = null;

    private CanvasGroup canvasGroup;
    
    public event Action OnJoinMatchQueueEventButton;
}
