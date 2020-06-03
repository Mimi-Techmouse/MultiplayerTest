using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class mt_bulletfly : MonoBehaviourPunCallbacks, IPunObservable
{
    public float speed = 10.0f;
    public Vector3 target;
    public mt_EventHandler Firer = null;
    public int myDamage = 5;
    public float timeCreated = 0;
    public int parentViewID = 0;
    public bool shouldDie = false;

    public void Awake() {
        timeCreated = Time.time;
    }

    public void Update() {

        if (shouldDie) {
            SafeRemove();
        }

    	if (Vector3.Distance(transform.position, target) < 20.0f) {
    		SafeRemove();
    	}

        if (Time.time-timeCreated > 10.0f) {
            SafeRemove();
        }

        //This will only happen if you are in fact not the 
        if (Firer == null && parentViewID > 0) {
            GameObject obj = PhotonView.Find(parentViewID).gameObject;
            Firer = obj.GetComponent<mt_EventHandler>();
        }

    }

    public void LateUpdate() {

        // Move our position a step closer to the target.
        float step =  speed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, target, step);

    }


    public void SetTarget(Vector3 pos) {

    	target = pos;

    }


    public void SetFirer(mt_EventHandler handler) {

        Firer = handler;
        //Debug.Log("assigning firer: "+handler.name);
        if (Firer.gameObject.GetComponent<PhotonView>() != null)
            parentViewID = Firer.gameObject.GetComponent<PhotonView>().ViewID;

        //Debug.Log("parent view id: "+parentViewID);

    }

    public void OnTriggerEnter(Collider other) {

        if (other.gameObject.GetComponent<mt_bulletfly>() != null) {
            Debug.Log("Hit fellow bullet");
            return;
        } if (Firer == null)  {
            Debug.Log("I don't have a firer yet");
            return;
        } if (Firer.gameObject == other.gameObject) {
            if (other.gameObject.GetComponent<PhotonView>() != null) {
                if (parentViewID == other.gameObject.GetComponent<PhotonView>().ViewID) {
                    Debug.Log("I hit myself!");
                    return;
                } else {
                    Debug.Log("hit ID: "+other.gameObject.GetComponent<PhotonView>().ViewID);
                }
            } else {
                Debug.Log("Other self doesn't have photon view.");
                return;
            }
        }

        shouldDie = true;

        Debug.Log(other.gameObject.name+" trigger entered!");

        mt_Constants.Damage damageToSend = new mt_Constants.Damage(myDamage, transform.position, Firer.gameObject);
        other.gameObject.SendMessage ("ApplyDamage", myDamage, SendMessageOptions.DontRequireReceiver);
        vp_Timer.In(0.5f, () => {SafeRemove();});

    }

    public void OnCollisionEnter(Collision other) {

        if (other.gameObject.GetComponent<mt_bulletfly>() != null) {
            Debug.Log("Hit fellow bullet");
            return;
        } if (Firer == null)  {
            Debug.Log("I don't have a firer yet");
            return;
        } if (Firer.gameObject == other.gameObject) {
            if (other.gameObject.GetComponent<PhotonView>() != null) {
                if (parentViewID == other.gameObject.GetComponent<PhotonView>().ViewID) {
                    Debug.Log("I hit myself!");
                    return;
                } else {
                    Debug.Log("hit ID: "+other.gameObject.GetComponent<PhotonView>().ViewID);
                }
            } else {
                Debug.Log("Other self doesn't have photon view.");
                return;
            }
        }

        shouldDie = true;

        Debug.Log(other.gameObject.name+" collider entered!");

        mt_Constants.Damage damageToSend = new mt_Constants.Damage(myDamage, transform.position, Firer.gameObject);
        other.gameObject.SendMessage ("ApplyDamage", damageToSend, SendMessageOptions.DontRequireReceiver);
        vp_Timer.In(0.5f, () => {SafeRemove();});

    }

    public void SafeRemove() {
        if (this != null && gameObject != null) {
            vp_Utility.Destroy(gameObject);
        }
    }

    /// <summary>
    /// Networking section!
    /// </summary>
    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            // We own this player: send the others our data
            stream.SendNext(parentViewID);
            stream.SendNext(target);
            stream.SendNext(shouldDie);
        } else {
            int viewID = (int)stream.ReceiveNext();
            if (viewID <= 0)
                return;

            this.parentViewID = viewID;
            this.target = (Vector3)stream.ReceiveNext();
            this.shouldDie = (bool)stream.ReceiveNext();
        }
    }
    #endregion
}
