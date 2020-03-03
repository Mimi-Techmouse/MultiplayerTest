using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_bulletfly : MonoBehaviour
{
    public float speed = 10.0f;
    public Vector3 target;

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
}
