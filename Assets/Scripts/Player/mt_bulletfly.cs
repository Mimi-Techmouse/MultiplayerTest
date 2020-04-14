using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_bulletfly : MonoBehaviour
{
    public float speed = 10.0f;
    public Vector3 target;
    public mt_EventHandler Firer = null;
    public int myDamage = 5;

    public void Update() {

    	if (Vector3.Distance(transform.position, target) < 1.0f) {
    		vp_Utility.Destroy(gameObject);
    	}

    }

    public void LateUpdate() {

        // Move our position a step closer to the target.
        //float step =  speed * Time.deltaTime; // calculate distance to move
        //transform.position = Vector3.MoveTowards(transform.position, target, step);

    }


    public void SetTarget(Vector3 pos) {

    	target = pos;

        Vector3 dir = (pos - transform.position).normalized;
        GetComponent<Rigidbody>().AddForce(dir*speed, ForceMode.VelocityChange);

    }


    public void SetFirer(mt_EventHandler handler) {

        Firer = handler;
        Debug.Log("assigning firer: "+handler.name);

    }

    public void OnTriggerEnter(Collider other) {

        if (other.gameObject.GetComponent<mt_bulletfly>() != null) {
            return;
        } if (Firer == null)  {
            return;
        } if (Firer.gameObject == other.gameObject) {
            return;
        }

        Debug.Log(other.gameObject.name+" trigger entered!");

        other.gameObject.SendMessage ("ApplyDamage", myDamage, SendMessageOptions.DontRequireReceiver);
        vp_Utility.Destroy(gameObject);

    }

    public void OnCollisionEnter(Collision other) {

        if (other.gameObject.GetComponent<mt_bulletfly>() != null) {
            Debug.Log("you just hit another bullet");
            return;
        } if (Firer == null)  {
            Debug.Log("you don't have a Firer yet");
            return;
        } if (Firer.gameObject == other.gameObject) {
            Debug.Log("you hit yourself!");
            return;
        }

        Debug.Log(other.gameObject.name+" collider entered!");

        mt_Constants.Damage damageToSend = new mt_Constants.Damage(myDamage, transform.position, Firer.gameObject);
        other.gameObject.SendMessage ("ApplyDamage", damageToSend, SendMessageOptions.DontRequireReceiver);
        vp_Utility.Destroy(gameObject);

    }
}
