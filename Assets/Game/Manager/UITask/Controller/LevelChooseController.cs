using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelChooseController : MonoBehaviour
{
    public void Open()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        _animator = GetComponent<Animator>();
        _animator.SetBool(OpenParameterId, true);
    }

    public Button[] GetButtons()
    {

        if (gameObject.transform.Find("BG_not_ui_Panel/Scroll View/Viewport/Content").childCount == 0)
            return null;
        else
        return gameObject.transform.Find("BG_not_ui_Panel/Scroll View/Viewport/Content").GetComponentsInChildren<Button>();
    }

    public void OnDestroy()
    {
        DestroyImmediate(GameObject.Find("LevelItem(Clone)+"));
        
    }
    
    public void Closed()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;

        _animator = GetComponent<Animator>();
        _animator.SetBool(OpenParameterId, false);
        datacount = 0;
    }


    public void OnOpenLevelDetail()
    {

        OnOpenLevelDetailButton?.Invoke();
    }

    public void OnCancel()
    {
        OnCancelButton?.Invoke();
    }


    public GameObject LevelItem;
    private GameObject Item_parent;

    private Text[] texts;
    private Button[] buttons;
    private CanvasGroup canvasGroup;

    private Animator _animator;
    const string OpenTransitionName = "Open";
    private readonly int OpenParameterId = Animator.StringToHash(OpenTransitionName);


    public event Action OnOpenLevelDetailButton;
    public event Action OnCancelButton;

    private int datacount = 0;
}
