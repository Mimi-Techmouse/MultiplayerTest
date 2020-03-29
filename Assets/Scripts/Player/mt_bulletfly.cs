using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_bulletfly : MonoBehaviour
{
    public float speed = 10.0f;
    public Vector3 target;
    public vp_StateEventHandler Firer = null;
    public float myDamage = 5.0f;

    public void Update() {

    	if (Vector3.Distance(transform.position, target) < 1.0f) {
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


    public void SetFirer(vp_StateEventHandler handler) {

        Firer = handler;

    }

    public void OnTriggerEnter(Collider other) {
        
        Debug.Log(this.name+" trigger entered!");
        other.gameObject.SendMessage ("ApplyDamage", myDamage, SendMessageOptions.DontRequireReceiver);
        vp_Utility.Destroy(gameObject);

    }

    public void OnCollisionEnter(Collision collision) {
        Debug.Log(this.name+" collider entered!");
    }
}
