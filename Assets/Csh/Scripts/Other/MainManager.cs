using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    private static MainManager _instance;

    public static MainManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MainManager();
            }
            return _instance;
        }
    }

    public void NetUnConnected()
    {

    }

    public bool LoginCheck(string account ,string password)
    {
        if (account == "11" && password == "22")
        {
            Debug.Log("the anwser of check is yes");
            UISystemManager.Instance.PopPanel();
            return true;
        }
        else
        {
            Debug.Log("the anwser of check is no");
            Go_Message("登录失败");
            return false;
        }
    }

    public void SignupCheck(string account, string password,string password2)
    {
        
    }


    public void ClearInput(GameObject gameObject)
    {

        InputField[] inputFields = gameObject.GetComponentsInChildren<InputField>();
        foreach (InputField i in inputFields)
        {
            i.text = null;
            Debug.Log("已清空"+i);
        }
    }

    public void Go_Message(string str)
    {
        if (GameObject.Find("MessageWindow(Clone)") == null)
        {
            Debug.Log("没有生成 MessageWindow(Clone)");
            return;
        }
        GameObject.Find("MessageWindow(Clone)").GetComponent<MessageManager>().Open();
        GameObject.Find("MessageWindow(Clone)").GetComponent<MessageManager>().ChangeContent(str);
    }
}
