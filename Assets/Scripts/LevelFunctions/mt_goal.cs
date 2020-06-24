using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_goal : MonoBehaviour
{

	/// *********************************************************************************** /// 
	/// On Trigger enter
	/// *********************************************************************************** /// 
	public void OnTriggerEnter(Collider other) {

        Debug.Log(other.transform.parent.name+" trigger entered!");

    }

	/// *********************************************************************************** /// 
	/// On collision enter
	/// *********************************************************************************** /// 
    public void OnCollisionEnter(Collision other) {

        Debug.Log(other.gameObject.name+" collider entered!");

    }
}
