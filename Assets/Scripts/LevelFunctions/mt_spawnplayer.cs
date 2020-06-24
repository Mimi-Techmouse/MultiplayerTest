using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_spawnplayer : MonoBehaviour
{
    public GameObject PlayerPrefab = null;
    public GameObject[] SpawnPoints;

    private void Start() {
    	GameObject player = PhotonNetwork.Instantiate("Prefabs/"+PlayerPrefab.name, transform.position, Quaternion.identity);

    	float thing = (player.GetComponent<PhotonView>().ViewID/1000.0f);
    	int id = Mathf.FloorToInt(thing)-1;
    	if (SpawnPoints.Length > id) {
    		player.transform.position = SpawnPoints[id].transform.position;
    		player.transform.rotation = SpawnPoints[id].transform.rotation;
    		Debug.Log("assigning position: "+SpawnPoints[id].name);

            SpawnPoints[id].transform.parent.GetComponentInChildren<mt_goaltrigger>().playerView = player.GetComponent<PhotonView>().ViewID;
    	} else {
    		Debug.Log("Don't have a spawn position; falling back on default.");
    	}
    }
}
