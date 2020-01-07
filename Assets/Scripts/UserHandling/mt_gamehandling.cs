using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

public class mt_gamehandling : MonoBehaviourPunCallbacks {

	public GameObject UI_PlayerPanel  	= null;
	public GameObject UI_ChatPanel 		= null;
	public GameObject UI_GamePanel 		= null;
	public GameObject UI_ButtonPanel 	= null;
	public GameObject UI_JoinedPanel 	= null;
	public mt_infopanel UI_InfoPanel 	= null;

	private mt_joinedgameinstance JoinedGameController = null;

	public GameObject UI_GameList = null;
	private List<mt_gameinstance> UI_GamesInstantiated = null;
	public GameObject UI_GamePrefab = null;

    private const string GameVersion = "0.1";
    private const int MaxPlayersPerRoom = 6;

    public List<RoomInfo> AvailableGames = null;	

	private string user_id;

    private void Awake() {
		user_id = PlayerPrefs.GetString("user_name");

    	PhotonNetwork.AutomaticallySyncScene = true;
    	PhotonNetwork.NickName = user_id;

    	if (!PhotonNetwork.IsConnected) {
    		PhotonNetwork.GameVersion = GameVersion;
    		PhotonNetwork.ConnectUsingSettings();
    	}

    	UI_GamesInstantiated = new List<mt_gameinstance>();
    }

    public void SetUpLobby() {
    	PhotonNetwork.JoinLobby();
    }

    public void CreateGame(string gameName) {
    	string[] roomCreator = new string[1];
    	roomCreator[0] = "Creator_Name";
    	RoomOptions roomOps = new RoomOptions() {
            IsVisible = true, IsOpen = true, MaxPlayers = MaxPlayersPerRoom, CustomRoomPropertiesForLobby = roomCreator
        };
        roomOps.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
        roomOps.CustomRoomProperties.Add("Creator_Name", user_id);

    	PhotonNetwork.CreateRoom(gameName, roomOps);
    }

    public void RedrawGames() {
    	if (AvailableGames == null)
    		return;

		//Add new elements if necessary
		while (UI_GamesInstantiated.Count < AvailableGames.Count) {
			GameObject newGame = Instantiate(UI_GamePrefab, UI_GameList.transform.position, Quaternion.identity);
			newGame.transform.SetParent(UI_GameList.transform);
			newGame.transform.localScale = new Vector3(1,1,1);
			UI_GamesInstantiated.Add(newGame.GetComponent<mt_gameinstance>());
		}

    	int n = 0;

		//Draw just the current list of players
		foreach (RoomInfo room in AvailableGames) { 
			UI_GamesInstantiated[n].gameObject.SetActive(true);
			UI_GamesInstantiated[n].GameName.text 		= room.Name;
			UI_GamesInstantiated[n].GameToJoin 			= room.Name;
			UI_GamesInstantiated[n].PlayerCount.text 	= room.PlayerCount+" / "+room.MaxPlayers;

			UI_GamesInstantiated[n].ToggleButton(room.IsOpen);

			n++;
		}

		//remove extra elements
		for (int i = n; i < UI_GamesInstantiated.Count; i++) {
			UI_GamesInstantiated[i].gameObject.SetActive(false);
		}
    }

    //Update the room details
    public void UpdateRoomDetails() {
    	if (JoinedGameController == null) {
    		JoinedGameController 				= UI_JoinedPanel.GetComponent<mt_joinedgameinstance>();
    	}

    	if (PhotonNetwork.CurrentRoom == null)
    		return;

    	RoomInfo room = PhotonNetwork.CurrentRoom;
    	JoinedGameController.UpdateDetails(room.Name, (string)room.CustomProperties["Creator_Name"], room.MaxPlayers, PhotonNetwork.PlayerList);
    }

    public void LeaveGame() {
    	if (PhotonNetwork.CurrentRoom == null)
    		return;

    	RoomInfo room = PhotonNetwork.CurrentRoom;
    	string sCreator = (string)room.CustomProperties["Creator_Name"];

    	//If you are the game creator, the game is over
    	if (sCreator == PhotonNetwork.NickName) {
    		//Later, inform everyone that the game is ending
    		//For the moment, just force everyone to quit
    		gameObject.SendMessage("EndGame");
    	}

    	bool success = PhotonNetwork.LeaveRoom(false);
    	if (success) {
    		StartCoroutine(CoShowMessage(1.0f, "Game Ended", "The host has ended the game.", "Game"));
    	}
    }

    public void StartGame() {
    	PhotonNetwork.CurrentRoom.IsOpen = false;
    	PhotonNetwork.LoadLevel("Game_TestLevel");
    } 

	private IEnumerator CoShowMessage(float fTime, string sTitle, string sMessage, string sReturnMode) {

    	UI_PlayerPanel.SetActive(false);
    	UI_ChatPanel.SetActive(false);
    	UI_GamePanel.SetActive(false);
    	UI_ButtonPanel.SetActive(false);
    	UI_JoinedPanel.SetActive(false);

    	UI_InfoPanel.TogglePanel(true, sTitle, sMessage);

		yield return new WaitForSeconds(fTime);

    	UI_InfoPanel.TogglePanel(false);

    	if (sReturnMode == "Game") {
    		UI_PlayerPanel.SetActive(true);
    		UI_GamePanel.SetActive(true);
    		UI_ButtonPanel.SetActive(true);
    	} else {
    		UI_PlayerPanel.SetActive(true);
    		UI_ChatPanel.SetActive(true);
    		UI_ButtonPanel.SetActive(true);
    	}

	}

    //**********************************
    //Overrides
    //**********************************
    public override void OnConnectedToMaster() {
    	Debug.Log("Connected to master");
    }

    public override void OnDisconnected(DisconnectCause cause) {
    	Debug.Log("Disconnected due to "+cause);
    }

    public override void OnJoinedLobby () {
    	Debug.Log("joined lobby!");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        Debug.Log(roomList.Count + " Rooms");
        base.OnRoomListUpdate(roomList);
        AvailableGames = roomList;
        RedrawGames();
    }

    public override void OnLeftLobby () {
    	Debug.Log("left lobby!");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {

    	Debug.Log("Failed to join room because "+message+" ("+returnCode+")");

    }

    public override void OnJoinedRoom() {
    	Debug.Log("Client successfully joined");
    	base.OnJoinedRoom();

    	gameObject.SendMessage("SwitchChat", PhotonNetwork.CurrentRoom.Name);

    	UI_PlayerPanel.SetActive(false);
    	UI_ChatPanel.SetActive(false);
    	UI_GamePanel.SetActive(false);
    	UI_ButtonPanel.SetActive(false);
    	UI_JoinedPanel.SetActive(true);

    	UpdateRoomDetails();
    }

    //Called when you leave the room
    public override void OnLeftRoom() {

    	base.OnLeftRoom();

    	gameObject.SendMessage("SwitchChat", "");

    	UpdateRoomDetails();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {

    	base.OnPlayerEnteredRoom(newPlayer);

    	UpdateRoomDetails();

    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {

    	base.OnPlayerLeftRoom(otherPlayer);

    	UpdateRoomDetails();

    }
}
