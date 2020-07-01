using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_spawnplayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject PlayerPrefab = null;
    public GameObject[] SpawnPoints;
    public Color[] PlayerColors;

    public GameObject[] ActivePlayers = null;

    public bool gameEnded = false;

    private void Start() {
    	GameObject player = PhotonNetwork.Instantiate("Prefabs/"+PlayerPrefab.name, transform.position, Quaternion.identity);

    	float thing = (player.GetComponent<PhotonView>().ViewID/1000.0f);
    	int id = Mathf.FloorToInt(thing)-1;
    	if (SpawnPoints.Length > id) {
    		player.transform.position = SpawnPoints[id].transform.position;
    		player.transform.rotation = SpawnPoints[id].transform.rotation;

            mt_EventHandler handler = player.GetComponent<mt_EventHandler>();
            handler.StartingLocation.Set(SpawnPoints[id].transform);
            
    		Debug.Log("assigning position: "+SpawnPoints[id].name);

            //SpawnPoints[id].transform.parent.GetComponentInChildren<mt_goaltrigger>().playerView = player.GetComponent<PhotonView>().ViewID;
            
    	} else {
    		Debug.Log("Don't have a spawn position; falling back on default.");
    	}

        ColorHaulers();

        vp_Timer.In(0.5f, () => { SetPlayerColors(); });
    }

    private void Update() {
        if (gameEnded) {
            Debug.Log("we should end the game!!!");

            foreach (GameObject player in ActivePlayers) {
                mt_PlayerEventHandler handler = player.GetComponent<mt_PlayerEventHandler>();
                if (handler.isVictorious.Get())
                    handler.ShowVictoryPanel.Send();
                else
                    handler.ShowLossPanel.Send();
            }
            //gameEnded = false;
        }
    }

    private void SetPlayerColors() {

        if (ActivePlayers != null)
            return;

        Debug.Log("doing player colors");

        ActivePlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in ActivePlayers) {

            float thing = (p.GetComponent<PhotonView>().ViewID/1000.0f);
            int id = Mathf.FloorToInt(thing)-1;
            SpawnPoints[id].transform.parent.GetComponentInChildren<mt_goaltrigger>().playerView = p.GetComponent<PhotonView>().ViewID;
            

            Transform objToColor = p.GetComponentInChildren<mt_shipbody>().bodyToColor;

            if (objToColor != null) {
                Debug.Log("found something to color! "+objToColor.name);
                Material mat = objToColor.GetComponent<MeshRenderer> ().material;
                Material newMat = new Material(mat);
                newMat.color = PlayerColors[id];
                objToColor.GetComponent<MeshRenderer> ().material = newMat;
            } else {
                Debug.Log("nothing to color "+p.name);
            }
        }
    }

    private void ColorHaulers() {
        for (int n = 0; n < SpawnPoints.Length; n++) {
            Debug.Log("going to the next hauler: "+n);
            Transform haulerModel = SpawnPoints[n].transform.parent.Find("ScavangerHauler_back");
            if (haulerModel == null) {
                Debug.Log("we failed to find the right model :(");
            }

            Material mat = haulerModel.GetComponent<MeshRenderer> ().material;
            Material newMat = new Material(mat);
            newMat.color = PlayerColors[n];
            haulerModel.GetComponent<MeshRenderer> ().material = newMat;
        }
    }

    /// <summary>
    /// Networking section!
    /// </summary>
    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            // We own this player: send the others our data
            stream.SendNext(gameEnded);
        } else {
            // Network player, receive data
            this.gameEnded = (bool)stream.ReceiveNext();
        }
    }
    #endregion
}
