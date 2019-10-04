using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginWindow : BasePanel
{
    private CanvasGroup canvasGroup;
    private Animator animator;
    private InputField[] inputFields;

    const string OpenTransitionName = "Open";
    private readonly int OpenParameterId = Animator.StringToHash(OpenTransitionName);
    
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
    }
    
    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;//当弹出新的面板的时候，让主菜单面板 不再和鼠标交互
    }

    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
    }

    public override void OnExit()
    {
        animator.SetBool(OpenParameterId, false);
    }

    public void OnPushPanel(string panelTypeString)
    {
        UIPanelType panelType = (UIPanelType)System.Enum.Parse(typeof(UIPanelType), panelTypeString);
        UISystemManager.Instance.PushPanel(panelType);
    }

    public void OnLogin()
    {
        inputFields = gameObject.GetComponentsInChildren<InputField>();

        if (MainManager.Instance.LoginCheck(inputFields[0].text, inputFields[1].text))
        {
            OnPushPanel("MainMenu");
        }

        MainManager.Instance.ClearInput(this.gameObject);
    }


}