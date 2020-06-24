using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_spawnplayer : MonoBehaviour
{
    public GameObject PlayerPrefab = null;
    public GameObject[] SpawnPoints;
    public Color[] PlayerColors;

    public GameObject[] ActivePlayers = null;

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

        ColorHaulers();
    }

    private void Update() {

        if (ActivePlayers != null)
            return;


        ActivePlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in ActivePlayers) {

            float thing = (p.GetComponent<PhotonView>().ViewID/1000.0f);
            int id = Mathf.FloorToInt(thing)-1;

            Transform objToColor = p.transform.Find("Fuselage");

            if (objToColor != null) {
                Material mat = objToColor.GetComponent<MeshRenderer> ().material;
                Material newMat = new Material(mat);
                newMat.color = PlayerColors[id];
                objToColor.GetComponent<MeshRenderer> ().material = newMat;
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
}
