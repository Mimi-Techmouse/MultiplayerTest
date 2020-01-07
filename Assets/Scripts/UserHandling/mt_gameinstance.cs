using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Photon.Pun;
using Photon.Realtime;

public class mt_gameinstance : MonoBehaviour
{
    public TMP_Text GameName 	= null;
    public TMP_Text PlayerCount = null;
    public Button 	JoinButton 	= null;
    public string 	GameToJoin 	= "";

    void Awake() {
    	JoinButton.interactable = false;
    }

    public void ToggleButton(bool isOpen) {

    	if (isOpen) {
    		JoinButton.interactable = true;
    		JoinButton.GetComponentInChildren<Text>().text = "Join";
    	} else {
    		JoinButton.interactable = false;
    		JoinButton.GetComponentInChildren<Text>().text = "Closed";
    	}
    }

    public void ClickButton() {
    	PhotonNetwork.JoinRoom(GameToJoin);
    }

}
