﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class mt_goal : MonoBehaviourPunCallbacks, IPunObservable
{

	public bool isGrabbed = false;

	/// *********************************************************************************** /// 
	/// On Trigger enter
	/// *********************************************************************************** /// 
	public void OnTriggerEnter(Collider other) {

		if (isGrabbed)
			return;

        Debug.Log(gameObject.name+": "+other.transform.parent.name+" trigger entered!");
        GameObject target = GetQuestRunnerObject(other.gameObject);

		target.SendMessage ("GrabMcGuffin", gameObject, SendMessageOptions.DontRequireReceiver);

    }

	/// *********************************************************************************** /// 
	/// On collision enter
	/// *********************************************************************************** /// 
    public void OnCollisionEnter(Collision other) {

		if (isGrabbed)
			return;

        Debug.Log(gameObject.name+": "+other.gameObject.name+" collider entered!");
        GameObject target = GetQuestRunnerObject(other.gameObject);

		target.SendMessage ("GrabMcGuffin", gameObject, SendMessageOptions.DontRequireReceiver);

    }

    /// *********************************************************************************** /// 
    /// Get that damage handler!
    /// *********************************************************************************** /// 
    public GameObject GetQuestRunnerObject(GameObject other) {

        if (other.GetComponent<mt_questrunner>() != null) {
            return other;
        }
        if (other.transform.parent.gameObject.GetComponent<mt_questrunner>() != null) {
            return other.transform.parent.gameObject;
        }

        return null;
    }

    /// <summary>
    /// Networking section!
    /// </summary>
    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    	if (stream.IsWriting) {
		    // We own this player: send the others our data
            stream.SendNext(isGrabbed);

		} else {
		    this.isGrabbed = (bool)stream.ReceiveNext();
		}
    }
    #endregion
}
