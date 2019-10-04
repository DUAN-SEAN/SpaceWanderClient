using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignupPanel : BasePanel
{
    private CanvasGroup canvasGroup;
    private InputField[] inputFields;

    public override void OnEnter()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public override void OnExit()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnSignup()
    {
        inputFields = gameObject.GetComponentsInChildren<InputField>();

        MainManager.Instance.SignupCheck(inputFields[0].text, inputFields[1].text, inputFields[2].text);

        MainManager.Instance.ClearInput(this.gameObject);
    }

    public void OnClosePanel()
    {
        MainManager.Instance.ClearInput(this.gameObject);

        UISystemManager.Instance.PopPanel();
    }
    
}
