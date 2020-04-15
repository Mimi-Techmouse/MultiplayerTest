using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class mt_bulletfly : MonoBehaviour
{
    public float speed = 10.0f;
    public Vector3 target;
    public mt_EventHandler Firer = null;
    public int myDamage = 5;
    public float timeCreated = 0;
    public int parentViewID = 0;

    public void Awake() {
        timeCreated = Time.time;
    }

    public void Update() {

    	if (Vector3.Distance(transform.position, target) < 1.0f) {
    		vp_Utility.Destroy(gameObject);
    	}

        if (Time.time-timeCreated > 10.0f) {
            vp_Utility.Destroy(gameObject);
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
        Debug.Log("assigning firer: "+handler.name);
        if (Firer.gameObject.GetComponent<PhotonView>() != null)
            parentViewID = Firer.gameObject.GetComponent<PhotonView>().ViewID;

        Debug.Log("parent view id: "+parentViewID);

    }

    public void OnTriggerEnter(Collider other) {

        if (other.gameObject.GetComponent<mt_bulletfly>() != null) {
            return;
        } if (Firer == null)  {
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

        Debug.Log(other.gameObject.name+" trigger entered!");

        other.gameObject.SendMessage ("ApplyDamage", myDamage, SendMessageOptions.DontRequireReceiver);
        vp_Utility.Destroy(gameObject);

    }

    public void OnCollisionEnter(Collision other) {

        if (other.gameObject.GetComponent<mt_bulletfly>() != null) {
            return;
        } if (Firer == null)  {
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

        Debug.Log(other.gameObject.name+" collider entered!");

        mt_Constants.Damage damageToSend = new mt_Constants.Damage(myDamage, transform.position, Firer.gameObject);
        other.gameObject.SendMessage ("ApplyDamage", damageToSend, SendMessageOptions.DontRequireReceiver);
        vp_Utility.Destroy(gameObject);

    }
}
