using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gui_login : MonoBehaviour
{
    public InputField nameField = null;
    public Button continueButton = null;

    private const string PlayerPrefsNameKey = "PlayerName";

    private void Start() {
    	SetUpInputField();
    }

    private void SetUpInputField() {

    	if (!PlayerPrefs.HasKey(PlayerPrefsNameKey)) return;

    	string playerName = PlayerPrefs.GetString(PlayerPrefsNameKey);

    	SetPlayerName(playerName);

    }

    public void SetPlayerName(string sName = "") {

    	continueButton.interactable = !string.IsNullOrEmpty(sName);

    }

    public void SavePlayerName() {
    	string sName = nameField.text; 

    	PhotonNetwork.NickName = sName;

    	PlayerPrefs.SetString(PlayerPrefsNameKey, sName);
    }
}
