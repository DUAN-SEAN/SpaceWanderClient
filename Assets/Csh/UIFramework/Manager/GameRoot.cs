using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UISystemManager.Instance.PushPanel(UIPanelType.LoginWindow);

        UISystemManager.Instance.OtherPanel(UIPanelType.MessageWindow);
    }
}
