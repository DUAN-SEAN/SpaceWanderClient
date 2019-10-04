using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageWindowController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Animator animator;
    private Text content;

    const string OpenTransitionName = "Open";
    private readonly int OpenParameterId = Animator.StringToHash(OpenTransitionName);
    

    public void Open(string messageContent)
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();

        gameObject.transform.SetAsLastSibling();
        canvasGroup.alpha = 1;
        animator.Play("Open", 0, 0);
        ChangeContent(messageContent);
    }

    public void ChangeContent(string messageContent)
    {
        content = gameObject.GetComponentInChildren<Text>();
        content.text = messageContent;
    }

    public void TobeClosed()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }
}
