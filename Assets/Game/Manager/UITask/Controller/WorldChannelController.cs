using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class WorldChannelController : MonoBehaviour
{

    public void Open(List<string> playerInfoList)
    {
        playerList = new List<string>();
        playerList = playerInfoList;
        searchPlayerList = new List<string>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;

        //datacount = 0;
        //for (int i = 0; i < playerList.Count; i++)
        //{
        //    AddPlayerItem(playerInfoList);
        //}

    }

    public void Closed()
    {
        gameObject.transform.Find("WorldChannelPanel/Search/SearchInputField").GetComponent<InputField>().text = null;
        //canvasGroup = GetComponent<CanvasGroup>();
        //canvasGroup.alpha = 0;
        //canvasGroup.blocksRaycasts = false;
        
        datacount = 0;
        Destroy(GameObject.Find("WorldChannel(Clone)"));
    }

    public int WeatherContainsPlayer(List<string> tempList)
    {
        string str = null;
        str = gameObject.transform.Find("WorldChannelPanel/Search/SearchInputField/Text").GetComponent<Text>().text;
        Debug.Log("You have Searched :" + str);
        if (tempList.Contains(str))
        {
            return tempList.FindIndex(item => item.Equals(str));
        }
        else
        {
            return -1;
        }
    }

    //public void OnSearchPlayer()
    //{
    //    datacount = 0;
    //    string str = null;
    //    str = gameObject.transform.Find("WorldChannelPanel/Search/SearchInputField/Text").GetComponent<Text>().text;
    //    Debug.Log("You have Searched :"+str);
    //    if (playerList.Contains(str))
    //    {
    //        searchPlayerList.Add(playerList[playerList.FindIndex(item => item.Equals(str))]);
    //        AddPlayerItem(searchPlayerList);
    //    }
    //    else
    //    {
    //        Debug.Log("查无此人");
    //    }
    //}

    public void OnDestroy()
    {
        DestroyImmediate(GameObject.Find("PlayerItem(Clone)+"));
    }

   

    //public void AddPlayerItem(List<string> list)
    //{
    //    texts = gameObject.transform.Find("WorldChannelPanel/Scroll View/Viewport/Content/PlayerItem(Clone)/Content").GetComponentsInChildren<Text>();
        
    //    foreach (Text text in texts)
    //    {
    //        text.text = list[datacount];
    //        datacount++;
    //    }
    //}

    //public void InstantiatePlayerItem()
    //{
    //    Item_parent = GameObject.Find("WorldChannel(Clone)/WorldChannelPanel/Scroll View/Viewport/Content");
    //    GameObject playerItem = Instantiate(PlayerItem);
    //    gbList.Add(playerItem);
    //    playerItem.transform.parent = Item_parent.transform;
    //    playerItem.transform.localScale = new Vector3(1f, 1f, 1f);
    //    texts = gameObject.transform.Find("WorldChannelPanel/Scroll View/Viewport/Content/PlayerItem(Clone)/Content").GetComponentsInChildren<Text>();
    //    GameObject.Find("PlayerItem(Clone)").name = "PlayerItem(Clone)+";
    //}

    public void OnInvite()
    {
        OnInviteButton?.Invoke();
    }

    public void OnCancel()
    {
        OnCancelButton?.Invoke();
    }

    public void OnSearchPlayer()
    {
        OnSearchPlayerButton?.Invoke();
    }

    private List<string> playerList;
    private List<string> searchPlayerList;

    private Text[] texts;

    //public List<GameObject> gbList;

    public GameObject PlayerItem;
    private GameObject Item_parent;

    private int datacount = 0;

    public Action OnInviteButton;
    public Action OnCancelButton;
    public Action OnSearchPlayerButton;

    private CanvasGroup canvasGroup;
}
