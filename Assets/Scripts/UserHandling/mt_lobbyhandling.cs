using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;

public class mt_lobbyhandling : MonoBehaviour, IChatClientListener {

	public ChatClient chatClient;

	private Dictionary<string, int> PlayerList;

	private string user_id;
	private string worldChat = "WORLD_CHAT"; //the channel

	void Start() {
		PlayerList = new Dictionary<string, int>();
		user_id = PlayerPrefs.GetString("user_name");
		Debug.Log("connecting with "+user_id);

		chatClient = new ChatClient(this);
		chatClient.UseBackgroundWorkerForSending = true;
		chatClient.ChatRegion = "US";
		chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(user_id));
	}

	void Update() {
		if (chatClient != null) {
			chatClient.Service(); 
		}
		PublishUserList();
	}

	public void PublishUserList() {
		string playerList = "";
		Dictionary<string, int>.KeyCollection users = PlayerList.Keys;
		foreach( string user in users ) {
			playerList += user+", ";
		}
		Debug.Log(playerList);
	}

	//***************************************
	//Things we have to implement:
	//***************************************
	//When we connect
	public void OnConnected() {
		Debug.Log("user connected.");
		chatClient.Subscribe(new string[]{ worldChat });
		chatClient.SetOnlineStatus(ChatUserStatus.Online);
	}

	//When we disconnect
	public void OnDisconnected() {
		Debug.Log("user disconnected.");
	}

	//General messages
	public void OnGetMessages(string channelName, string[] senders, object[] messages) {
		for(int i = 0; i < senders.Length; i++) {

			//Make sure the player list is current
			string sUser = senders[i];
			string sMessage = (string)messages[i];
			if (sMessage == "Joined" && !PlayerList.ContainsKey(sUser)) {
				PlayerList.Add(sUser, 1);
			}
			if (sMessage == "Left" && PlayerList.ContainsKey(sUser)) {
				PlayerList.Remove(sUser);
			}

			string d = senders[i] + ": " + messages[i];
			Debug.Log(d);
		}

		// Scroll to bottom
		//StartCoroutine(CoScrollToBottom());
	}

	//Private message
	public void OnPrivateMessage(string sender, object message, string channelName) { 
		Debug.Log("User recieved private message.");
	}

	//On subscribing
	public void OnSubscribed(string[] channels, bool[] results) {
		chatClient.PublishMessage(worldChat, "Joined");
		Debug.Log("User subscribed.");
	}

	//On user subscribed
	public void OnUserSubscribed(string str1, string str2) {
		Debug.Log("User subscribed: "+str1+", "+str2);
	}

	public void OnUnsubscribed(string[] channels) { 
		Debug.Log("User unsubscribed.");
	}

	//On user subscribed
	public void OnUserUnsubscribed(string str1, string str2) {
		Debug.Log("User unsubscribed: "+str1+", "+str2);
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
		Debug.Log("Status update done");
	}

	public void OnChatStateChange(ChatState state) {
		Debug.Log("chat state changed");
	}

	public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message) {
		Debug.Log(message);
	}

	void OnApplicationQuit() {
		Debug.Log("quitting!");
		if(chatClient != null) {
			chatClient.Disconnect();
		}
	}
}