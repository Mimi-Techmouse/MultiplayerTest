using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class mt_goaltrigger : MonoBehaviourPunCallbacks, IPunObservable
{
	public int playerView = -1;

    /// *********************************************************************************** /// 
	/// On Trigger enter
	/// *********************************************************************************** /// 
	public void OnTriggerEnter(Collider other) {

       // Debug.Log(gameObject.name+": "+other.gameObject.name+" trigger entered!");
        GameObject target = GetQuestRunnerObject(other.gameObject);
        if (target == null)
            return;

		target.SendMessage ("McGuffinDelivered", playerView, SendMessageOptions.DontRequireReceiver);

    }

	/// *********************************************************************************** /// 
	/// On collision enter
	/// *********************************************************************************** /// 
    public void OnCollisionEnter(Collision other) {

      //  Debug.Log(gameObject.name+": "+other.gameObject.name+" collider entered!");
        GameObject target = GetQuestRunnerObject(other.gameObject);
        if (target == null)
            return;

		target.SendMessage ("McGuffinDelivered", playerView, SendMessageOptions.DontRequireReceiver);

    }

    /// *********************************************************************************** /// 
    /// Get that damage handler!
    /// *********************************************************************************** /// 
    public GameObject GetQuestRunnerObject(GameObject other) {

        if (other.GetComponent<mt_questrunner>() != null) {
            return other;
        }
        if (other.transform.parent != null && other.transform.parent.gameObject.GetComponent<mt_questrunner>() != null) {
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
            stream.SendNext(playerView);

		} else {
		    this.playerView = (int)stream.ReceiveNext();
		}
    }
    #endregion
}
