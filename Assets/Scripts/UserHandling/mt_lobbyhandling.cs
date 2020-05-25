using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Photon.Chat;
using Photon.Realtime;
using Photon.Pun;

using TMPro;

public class mt_lobbyhandling : MonoBehaviour, IChatClientListener {

	public ChatClient chatClient;

	public GameObject UI_PlayerList = null;
	private List<GameObject> UI_PlayersInstantiated = null;
	public GameObject UI_PlayerPrefab = null;

	public GameObject UI_MessageList = null;
	public ScrollRect UI_MessageScroller = null;
	private List<GameObject> UI_MessagesInstantiated = null;
	public GameObject UI_MessagePrefab = null;
	public TMP_InputField UI_MessageInput = null;

	public GameObject UI_RoomChatList 		= null;
	public ScrollRect UI_RoomChatScroller 	= null;
	public TMP_InputField UI_RoomChatInput 	= null;


	private Dictionary<string, int> PlayerList;

	private string user_id;
	private string worldChat 		= "MultiplayerTest_General"; //the channel
	private string currentChat 		= "";

	void Start() {

		PlayerList = new Dictionary<string, int>();
		user_id = PlayerPrefs.GetString("user_name");
		Debug.Log("connecting with "+user_id);

		chatClient = new ChatClient(this);
		chatClient.UseBackgroundWorkerForSending = true;
		chatClient.ChatRegion = "US";
		chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(user_id));

		UI_PlayersInstantiated = new List<GameObject>();
		UI_MessagesInstantiated = new List<GameObject>();

		currentChat = worldChat;

	}

	void Update() {
		vp_Utility.LockCursor = false;
        Screen.lockCursor = false;
        Cursor.visible = true;

		if (chatClient != null) {
			chatClient.Service(); 
		}
	}

	public void AddPlayer(string sUser) {
		PlayerList.Add(sUser, ChatUserStatus.Online);
		string[] list = new string[1];
		list[0] = sUser;
		chatClient.AddFriends(list);

		RedrawPlayerList();
	}

	public void RedrawPlayerList() {
		int n = 0;

		//Add new elements if necessary
		while (UI_PlayersInstantiated.Count < PlayerList.Count) {
			GameObject newPlayer = Instantiate(UI_PlayerPrefab, UI_PlayerList.transform.position, Quaternion.identity);
			newPlayer.transform.SetParent(UI_PlayerList.transform);
			newPlayer.transform.localScale = new Vector3(1,1,1);
			UI_PlayersInstantiated.Add(newPlayer);
		}

		//Draw just the current list of players
		foreach (KeyValuePair<string, int> i in PlayerList) { 
			UI_PlayersInstantiated[n].gameObject.SetActive(true);
			UI_PlayersInstantiated[n].GetComponent<TMP_Text>().text = i.Key;
			n++;
		}

		//remove extra elements
		for (int i = n; i < UI_PlayersInstantiated.Count; i++) {
			UI_PlayersInstantiated[i].gameObject.SetActive(false);
		}
	}

	string[] GetPlayerList() {
		string[] list = new string[PlayerList.Count];
		int n = 0;
		foreach (KeyValuePair<string, int> i in PlayerList) { 
			Debug.Log("Online: "+i.Key);
            list[n] = i.Key;
            n++;
        }

        return list;
	}

	public void AddMessage(string msg) {
		GameObject newMessage = null; 

		if (currentChat == worldChat) {
			newMessage = Instantiate(UI_MessagePrefab, UI_MessageList.transform.position, Quaternion.identity);
			newMessage.transform.SetParent(UI_MessageList.transform);
		} else {
			newMessage = Instantiate(UI_MessagePrefab, UI_RoomChatList.transform.position, Quaternion.identity);
			newMessage.transform.SetParent(UI_RoomChatList.transform);
		}

		newMessage.transform.localScale = new Vector3(1,1,1);
		newMessage.GetComponent<TMP_Text>().text = msg;
		UI_MessagesInstantiated.Add(newMessage);
	}

	public void SendMessageInput() {
		string msg = "";

		if (currentChat == worldChat) {
		 	msg = UI_MessageInput.text;
			UI_MessageInput.text = "";
		} else {
		 	msg = UI_RoomChatInput.text;
			UI_RoomChatInput.text = "";
		 }

		chatClient.PublishMessage(currentChat, msg);
	}

	//For switching chats when you join a game
	public void SwitchChat(string channel = "") {
		if (channel == "")
			channel = worldChat;

		currentChat = channel;
		Debug.Log("switching chat to "+channel);
		chatClient.Subscribe(new string[]{ currentChat });
		chatClient.SetOnlineStatus(ChatUserStatus.Online);
	}

	public void EndGame() {
		//We're not in a game chat, skip this command
		if (currentChat == worldChat) 
			return;

		chatClient.PublishMessage(currentChat, "//EndGame");
	}

	private IEnumerator CoScrollToBottom() {
		yield return new WaitForSeconds(0.1f);
		UI_MessageScroller.normalizedPosition = new Vector2(0, 0);
		UI_RoomChatScroller.normalizedPosition = new Vector2(0, 0);
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

		if (channelName != currentChat)
			return;

		for(int i = 0; i < senders.Length; i++) {

			//Make sure the player list is current
			string sUser = senders[i];
			string sMessage = (string)messages[i];
			if (!PlayerList.ContainsKey(sUser)) {
				AddPlayer(sUser);
			} else {
				PlayerList[sUser] = ChatUserStatus.Online;
			}

			if (sMessage == "//EndGame") {
				gameObject.SendMessage("LeaveGame");
			}

			if (sMessage == "//Ping" || sMessage == "//EndGame")
				continue;

			string msg = senders[i] + ": " + messages[i];
			if (sMessage != "")
				AddMessage(msg);
		}

		// Scroll to bottom
		StartCoroutine(CoScrollToBottom());
	}

	//Private message
	public void OnPrivateMessage(string sender, object message, string channelName) { 
		//Debug.Log("User recieved private message.");
	}

	//On subscribing
	public void OnSubscribed(string[] channels, bool[] results) {
		chatClient.PublishMessage(currentChat, "Joined");
		//Debug.Log("User subscribed.");
	}

	//On user subscribed
	public void OnUserSubscribed(string str1, string str2) {
		//Debug.Log("User subscribed: "+str1+", "+str2);
	}

	public void OnUnsubscribed(string[] channels) { 
		//Debug.Log("User unsubscribed.");
	}

	//On user subscribed
	public void OnUserUnsubscribed(string str1, string str2) {
		//Debug.Log("User unsubscribed: "+str1+", "+str2);
	}

	//We keep track of who is online by pinging the server every so often
	//If we stop recieving pings, then we assume you don't exist anymore.
	public void OnStatusUpdate(string user, int status, bool gotMessage, object message) {
		//Debug.Log("Status update: "+user+", "+status+", "+gotMessage+", "+message);
		chatClient.SetOnlineStatus(ChatUserStatus.Online);
		chatClient.PublishMessage(currentChat, "//Ping");

		if (PlayerList.ContainsKey(user) && status == ChatUserStatus.Offline) {
			PlayerList.Remove(user);
			RedrawPlayerList();
		} else if (!PlayerList.ContainsKey(user)) {
			AddPlayer(user);
		}
	}

	public void OnChatStateChange(ChatState state) {
		//Debug.Log("chat state changed");
	}

	public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message) {

		if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
		{
			Debug.LogError(message);
		}
		else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
		{
			Debug.LogWarning(message);
		}
		else
		{
			Debug.Log(message);
		}
	}

	void OnApplicationQuit() {
		//Debug.Log("quitting!");
		if(chatClient != null) {
			chatClient.Disconnect();
		}
	}
}