using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

using TMPro;

public class mt_joinedgameinstance : MonoBehaviour {

	public string GameName 			= "";
	public string GameCreator 		= "";
	public int maxPlayers 			= 0;
	public int playerCount 			= 0;
	public Player[] playersInRoom 	= null;

	public GameObject UI_PlayerList 				= null;
	private List<GameObject> UI_PlayersInstantiated = null;
	public GameObject UI_PlayerPrefab 				= null;

	public TMP_Text UI_GameName						= null;
	public TMP_Text UI_MaxPlayers					= null;
	public Button UI_StartButton 					= null;

	void Awake() {
		UI_PlayersInstantiated = new List<GameObject>();
	}

	public void UpdateDetails(string sName, string sCreator, int nCount, Player[] players) {
		GameName = sName;
		GameCreator = sCreator;
		maxPlayers = nCount;
		playerCount = players.Length;
		playersInRoom = players;

		UI_GameName.text = GameName;
		UI_MaxPlayers.text = playerCount+" of "+maxPlayers+" max";

		if (GameCreator != PhotonNetwork.NickName)
			UI_StartButton.interactable = false;
		else
			UI_StartButton.interactable = true;

		RedrawPlayerList();
	}
    

	public void RedrawPlayerList() {
		int n = 0;

		//Add new elements if necessary
		while (UI_PlayersInstantiated.Count < playerCount) {
			GameObject newPlayer = Instantiate(UI_PlayerPrefab, UI_PlayerList.transform.position, Quaternion.identity);
			newPlayer.transform.SetParent(UI_PlayerList.transform);
			newPlayer.transform.localScale = new Vector3(1,1,1);
			UI_PlayersInstantiated.Add(newPlayer);
		}

		//Draw just the current list of players
		foreach (Player player in playersInRoom) { 
			UI_PlayersInstantiated[n].gameObject.SetActive(true);
			UI_PlayersInstantiated[n].GetComponent<TMP_Text>().text = player.NickName;
			n++;
		}

		//remove extra elements
		for (int i = n; i < UI_PlayersInstantiated.Count; i++) {
			UI_PlayersInstantiated[i].gameObject.SetActive(false);
		}
	}
}
