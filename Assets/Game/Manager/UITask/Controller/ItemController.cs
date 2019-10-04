using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{

    public void OnChooseItem()
    {
        if(ShipHouseController._whichBeChosen.Equals("Ship"))
        _beChosenItem = gameObject.transform.Find("ContentText/ShipNameContentText").GetComponent<Text>().text;
        if (ShipHouseController._whichBeChosen.Equals("Weapon1")|| ShipHouseController._whichBeChosen.Equals("Weapon2"))
            _beChosenItem = gameObject.transform.Find("ContentText/SkillNameContentText").GetComponent<Text>().text;
        
        ChangeYourChoice();

        Destroy(GameObject.Find("AdaptedDetailPanel(Clone)"));
    }

    public void ChangeYourChoice()
    {
        switch (ShipHouseController._whichBeChosen)
        {
            case "Ship":
                GameObject.Find("ShipHousePanel(Clone)/TextContentGroup/ShipContentText").GetComponent<Text>().text = _beChosenItem;
                break;
            case "Weapon1":
                GameObject.Find("ShipHousePanel(Clone)/TextContentGroup/Weapon1ContentText").GetComponent<Text>().text = _beChosenItem;
                break;
            case "Weapon2":
                GameObject.Find("ShipHousePanel(Clone)/TextContentGroup/Weapon2ContentText").GetComponent<Text>().text = _beChosenItem;
                break;
            default:
                break;
        }
        
    }
    
    private string _beChosenItem = null;

}
