using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_spawnplayer : MonoBehaviour
{
    public GameObject PlayerPrefab = null;

    private void Start() {

    	Vector3 RandomOffset = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));

    	PhotonNetwork.Instantiate(PlayerPrefab.name, Vector3.zero+RandomOffset, Quaternion.identity);
    }
}
