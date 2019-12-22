using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gui_mainmenu : MonoBehaviourPunCallbacks {

    public GameObject findOpponentPanel = null;
    public GameObject waitingStatusPanel = null;
    public Text waitingStatusText = null;

    private bool isConnecting = false;
    private const string GameVersion = "0.1";
    private const int MaxPlayersPerRoom = 2;

    private void Awake() {
    	PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void FindOpponent() {

    	isConnecting = true;
    	findOpponentPanel.SetActive(false);
    	waitingStatusPanel.SetActive(true);

    	waitingStatusText.text = "Searching. . .";

    	if (PhotonNetwork.IsConnected) {
    		PhotonNetwork.JoinRandomRoom();
    	} else {
    		PhotonNetwork.GameVersion = GameVersion;
    		PhotonNetwork.ConnectUsingSettings();
    	}

    }

    public override void OnConnectedToMaster() {
    	Debug.Log("Connected to master");

    	if (isConnecting) {
    		PhotonNetwork.JoinRandomRoom();
    	}
    }

    public override void OnDisconnected(DisconnectCause cause) {
    	Debug.Log("Disconnected due to "+cause);

    	findOpponentPanel.SetActive(true);
    	waitingStatusPanel.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {

    	Debug.Log("No opponents are waiting, creating a new room");

    	PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = MaxPlayersPerRoom });

    }

    public override void OnJoinedRoom() {
    	Debug.Log("Client successfully joined");

    	int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

    	if (playerCount != MaxPlayersPerRoom) {
    		waitingStatusText.text = "Waiting for opponent. . .";
    		Debug.Log("Client waiting for opponent");
    	} else {
    		Debug.Log("Match is ready!");
    		waitingStatusText.text = "Opponent found.";
    	}
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {

    	int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

    	if (playerCount == MaxPlayersPerRoom) {
    		PhotonNetwork.CurrentRoom.IsOpen = false;

    		Debug.Log("Match is ready to begin");

    		waitingStatusText.text = "Opponent found.";

    		PhotonNetwork.LoadLevel("MainScene");
    	}
    }
}
