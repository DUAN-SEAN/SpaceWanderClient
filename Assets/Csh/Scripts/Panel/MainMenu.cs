using UnityEngine;
using System.Collections;

public class MainMenu : BasePanel
{

    private CanvasGroup canvasGroup;
    private Animator animator;

    const string OpenTransitionName = "Open";
    private readonly int OpenParameterId = Animator.StringToHash(OpenTransitionName);

    public override void OnEnter()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
        animator.SetBool(OpenParameterId, true);
    }

    public override void OnPause()
    {
        canvasGroup.blocksRaycasts = false;//当弹出新的面板的时候，让主菜单面板 不再和鼠标交互
    }
    public override void OnResume()
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPushPanel(string panelTypeString)
    {
        UIPanelType panelType = (UIPanelType)System.Enum.Parse(typeof(UIPanelType), panelTypeString);
        UISystemManager.Instance.PushPanel(panelType);
    }

}