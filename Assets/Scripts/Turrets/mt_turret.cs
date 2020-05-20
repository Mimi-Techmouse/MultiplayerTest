using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mt_turret : MonoBehaviour {

	public GameObject[] InstantiatedWeapons;

	void Start() {
		foreach (GameObject weapon in InstantiatedWeapons) {

    		mt_fireweapon w = weapon.GetComponent<mt_fireweapon>();
    		w.AnimationController.SetTrigger("Fire");

    	}
	}
}
