using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public void RefreshTeamGroup(Int32 i, List<String> list, ulong teamid)
    {
        player1 = GameObject.Find("TeamGroup/ItemLayoutGroup/ItemButton1/Player1_Text").GetComponent<Text>();
        player2 = GameObject.Find("TeamGroup/ItemLayoutGroup/ItemButton2/Player2_Text").GetComponent<Text>();
        player3 = GameObject.Find("TeamGroup/ItemLayoutGroup/ItemButton3/Player3_Text").GetComponent<Text>();
        player4 = GameObject.Find("TeamGroup/ItemLayoutGroup/ItemButton4/Player4_Text").GetComponent<Text>();

        GameObject.Find("TeamGroup/TeamId/TeamId_Content").GetComponent<Text>().text = teamid.ToString();

        switch (i)
        {
            case 1:
                player1.text = list[0];
                player2.text = null;
                player3.text = null;
                player4.text = null;
                break;
            case 2:
                player1.text = list[0];
                player2.text = list[1];
                player3.text = null;
                player4.text = null;
                break;
            case 3:
                player1.text = list[0];
                player2.text = list[1];
                player3.text = list[2];
                player4.text = null;
                break;
            case 4:
                player1.text = list[0];
                player2.text = list[1];
                player3.text = list[2];
                player4.text = list[3];
                break;
            default:
                player1.text = null;
                player2.text = null;
                player3.text = null;
                player4.text = null;
                break;
        }
    }

    public void SwitchBlocksRaycasts(bool b)
    {
        canvasGroup.blocksRaycasts = b;
    }

    public void SwitchButton1ToButton2()
    {
        //Debug.Log("B1 TO B2");
        SwitchButton1.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        SwitchButton1.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

        SwitchButton2.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        SwitchButton2.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void SwitchButton2ToButton1()
    {
        //Debug.Log("B2 TO B1");
        SwitchButton2.gameObject.GetComponent<CanvasGroup>().alpha = 0;
        SwitchButton2.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;

        SwitchButton1.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        SwitchButton1.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

    public void Open()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        _animator = GetComponent<Animator>();
        _animator.SetBool(OpenParameterId, true);
    }

    public void Closed()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        _animator = GetComponent<Animator>();
        _animator.SetBool(OpenParameterId, false);
    }

    public void OnLogout()
    {
        OnLogoutButton?.Invoke();
    }

    public void OnLevelChoose()
    {
        OnLevelChooseButton?.Invoke();
    }

    public void OnWorldChannel()
    {
        OnWorldChannelButton?.Invoke();
    }

    public void OnShipHouse()
    {
        OnShipHouseButton?.Invoke();
    }

    public void OnCreateTeam()
    {
        OnCreateTeamButton?.Invoke();
    }

    public void OnLeaveTeam()
    {
        OnLeaveTeamButton?.Invoke();
    }

    public void OnQuitMatching()
    {
        OnQuitMatchingButton?.Invoke();
    }

    public void OnStillBuilding()
    {
        OnStillBuildingButton?.Invoke();
    }

    public GameObject SwitchButton1;
    public GameObject SwitchButton2;

    private CanvasGroup canvasGroup;

    private Animator _animator;
    const string OpenTransitionName = "Open";
    private readonly int OpenParameterId = Animator.StringToHash(OpenTransitionName);

    private Text player1;
    private Text player2;
    private Text player3;
    private Text player4;

    public event Action OnLevelChooseButton;
    public event Action OnWorldChannelButton;
    public event Action OnShipHouseButton;
    public event Action OnCreateTeamButton;
    public event Action OnLeaveTeamButton;
    public event Action OnQuitMatchingButton;
    public event Action OnLogoutButton;

    public event Action OnStillBuildingButton;



    public void OnSwitchAudio()
    {
        OnSwitchAudioButton?.Invoke();
    }
    public event Action OnSwitchAudioButton;
}
