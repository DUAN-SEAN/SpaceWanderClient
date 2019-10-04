using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdaptedDetailController : MonoBehaviour
{

    public void Open(List<string> list, string str,int count)
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        datacount = 0;
        for (int i = 0; i < count; i++)
        {
            AddItem(list, str);
        }
    }

    public void Closed()
    {
        datacount = 0;
        Destroy(GameObject.Find("AdaptedDetailPanel(Clone)"));
    }

    public void AddItem(List<string> list,string ItemType)
    {
        InstantiateItem(ItemType);
        foreach(Text text in texts)
        {
            text.text = list[datacount];
            datacount++;
        }
    }

    public void InstantiateItem(string ItemType)
    {
        switch (ItemType)
        {
            case "ShipType":
                Item_parent = GameObject.Find("Scroll View/Viewport/ItemGroup");
                GameObject shipItem = Instantiate(ShipItem);
                shipItem.transform.parent = Item_parent.transform;
                shipItem.transform.localScale = new Vector3(1f, 1f, 1f);
                texts = gameObject.transform.Find("Scroll View/Viewport/ItemGroup/ShipItem(Clone)/ContentText").GetComponentsInChildren<Text>();
                GameObject.Find("ShipItem(Clone)").name = "ShipItem(Clone)+";
                break;
            case "SkillType":
                Item_parent = GameObject.Find("Scroll View/Viewport/ItemGroup");
                GameObject damageSkillItem = Instantiate(DamageSkillItem);
                damageSkillItem.transform.parent = Item_parent.transform;
                damageSkillItem.transform.localScale = new Vector3(1f, 1f, 1f);
                texts = gameObject.transform.Find("Scroll View/Viewport/ItemGroup/DamageSkillItem(Clone)/ContentText").GetComponentsInChildren<Text>();
                GameObject.Find("DamageSkillItem(Clone)").name = "DamageSkillItem(Clone)+";
                break;
            default:
                print("既不是Ship也不是DamageSkill");
                break;
        }
    }

    public void OnCancel()
    {
        OnCancelButton?.Invoke();
    }

    public Action OnCancelButton;

    private CanvasGroup canvasGroup;

    public GameObject ShipItem;
    public GameObject DamageSkillItem;
    //private GameObject SummonSkillItem;
    //private GameObject RecoverySkillItem;
    //private GameObject GainSkillItem;

    private GameObject Item_parent;

    private Text[] texts;
    
    private int datacount = 0;

}
