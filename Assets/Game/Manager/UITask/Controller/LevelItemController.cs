
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelItemController : MonoBehaviour
{

    public void GetChooseLevel()
    {

        BeChooseLevel = gameObject.transform.Find("Content/Level").GetComponent<Text>().text;
        BeChooseMemberCount = gameObject.transform.Find("Content/MemberCount").GetComponent<Text>().text;
    }

    public static string BeChooseLevel = null;
    public static string BeChooseMemberCount = null;

}
