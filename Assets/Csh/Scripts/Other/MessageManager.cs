using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class MessageManager : BasePanel
{
    private CanvasGroup canvasGroup;
    private Animator animator;
    private Text content;

    const string OpenTransitionName = "Open";
    private readonly int OpenParameterId = Animator.StringToHash(OpenTransitionName);
    
    public override void OnEnter()
    {
        animator = GetComponent<Animator>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Open()
    {
        gameObject.transform.SetAsLastSibling();
        canvasGroup.alpha = 1;
        animator.Play("Open", 0, 0);
    }

    public void ChangeContent(string messageContent)
    {
        content = gameObject.GetComponentInChildren<Text>();
        content.text = messageContent;
    }

    public void TobeClosed()
    {
        canvasGroup.alpha = 0;
    }
}